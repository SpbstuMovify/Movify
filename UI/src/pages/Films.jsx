import { useEffect, useState } from "react";
import Navigation from "../components/Navigation";
import './Films.css'
import { searchFilms, getPersonalList, addToPersonalList, removeFromPersonalList } from "../services/api";
import { useNavigate, useLocation } from "react-router-dom";
import { useForm } from "react-hook-form";
import { useAuth } from "../contexts/AuthContext";

function Films(){
    const {register, handleSubmit, formState: {errors}, setValue, setError} = useForm();
    const {userData} = useAuth();
    const navigate = useNavigate();
    const [films, setFilms] = useState();
    const [pageSize, setPageSize] = useState(5);
    const [pageNumber, setPageNumber] = useState(0);
    const [personalList, setPersonalList] = useState(null);

    const pageSizeArray = [3, 5, 10, 20, 50];
    const [maxPageNumber, setMaxPageNumber] = useState(100);
    const currentYear = new Date().getFullYear()

    const location = useLocation();
    const [queryParams, setQueryParams] = useState(new URLSearchParams(location.search));


    const mappingJSON = {
        "age_restriction": {
            "EIGHTEEN_PLUS": "18+",
            "SIXTEEN_PLUS": "16+",
            "TWELVE_PLUS": "12+",
            "SIX_PLUS": "6+",
            "ZERO_PLUS": "0+"
        }
    }

    const getPages = (filmsResponse) => {
        filmsResponse.content.forEach((film)=>{
            film.age_restriction = mappingJSON.age_restriction[film.age_restriction]
            let spacedString = film.genre.replace(/_/g, " ");
            film.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            spacedString = film.category.replace(/_/g, " ");
            film.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            film.publisher = film.publisher.replace(/_/g, " ");
        });
        setMaxPageNumber(filmsResponse.totalPages-1);
        setFilms(filmsResponse.content);
    }

    const getFilms = async () => {
        const filmsResponse = await searchFilms(pageSize, pageNumber, queryParams.get("year"), queryParams.get("genre"), 
        queryParams.get("title"), queryParams.get("age_restriction"), );
        getPages(filmsResponse);
    }
    const getUserPersonalList = async () => {
        try {
            const personalListResponse = await getPersonalList(userData.user_id, userData.token);
            setPersonalList(personalListResponse);
        }
        catch (error) {
            console.error(error.message);
        }
    }

    useEffect( () => {
        getFilms();
    },[pageNumber, pageSize]);

    useEffect(()=> {
        if (userData) {
            getUserPersonalList();
        }
    }, [userData])

    useEffect(() => {
        setValue("title", queryParams.get("title") || "");
        setValue("ageRestriction", queryParams.get("age_restriction") || "");
        setValue("genre", queryParams.get("genre") || "");
        setValue("year", queryParams.get("year") || "");
      }, [queryParams, setValue]);

    const handlePageBack = () => {
        if (pageNumber > 0)
        {
            setPageNumber(pageNumber-1);
        }
    }

    const handlePageForward = () => {
        if (pageNumber < maxPageNumber)
        {
            setPageNumber(pageNumber+1);
        }
    }

    const handleSearch = async (data) => {
        try {
            const response = await searchFilms(pageSize, pageNumber, data.year, data.genre, data.title, data.ageRestriction);
            getPages(response);
            const paramsJSON = JSON.parse(JSON.stringify({
                "title": data.title,
                "year": data.year,
                "genre": data.genre,
                "age_restriction": data.ageRestriction
            }, (key, value) => {
                return value === null || value === "" ? undefined : value;
            }));
            const newParams = new URLSearchParams(paramsJSON);
            setQueryParams(newParams);
            navigate(`/films?${newParams.toString()}`);
        }
        catch(error) {
            setError("root", "Server error has occured...");
            console.error(error.message);
        }
    }

    const handleAddToPersonalList = async (event, contentId) => {
        event.stopPropagation();
        try {
            const response = await addToPersonalList(userData.user_id, contentId, userData.token);
            setPersonalList([...personalList, {id: response.content_id}])
        }
        catch (error)
        {
            console.error(error.message);
        }
    }

    const handleRemoveFromPersonalList = async (event, contentId) => {
        event.stopPropagation();
        try {
            const response = await removeFromPersonalList(userData.user_id, contentId, userData.token);
            setPersonalList(personalList.filter((item)=>item.id !== contentId));
        }
        catch (error)
        {
            console.error(error.message);
        }
    }

    return <>
        <Navigation/>
        <div className="films-body">
            <div className="film-list">
                <h1 style={{textAlign: "center"}}>Watch all movies and series you want!</h1>
                <form onSubmit={handleSubmit(handleSearch)} className="search-bar-wrapper">
                    <input {...register("title")} className="search-bar" type="text" placeholder="Search..." />
                    <button className="image-button" type="submit">
                        <img src="/images/search_button.png"/>
                    </button>
                    {errors.root && (<label style={{color: "#CC0000", marginTop: "15px", marginBottom: "0"}}>
                        {errors.root.message}</label>)}
                </form>
                <div className="film-container">
                {films && films.map((film) => (
                    <div key={film.id} className="film-element" onClick={()=>navigate(`/films/${film.id}`)}>
                        <img className="film-logo" src={film.thumbnail ? film.thumbnail : "/images/no_image.jpg"} />
                        <div className="film-info">
                            <div className="film-element-header">
                                <h2 className="film-title">{film.title}</h2>
                                <div style={{display: "flex", gap: "10px"}}>
                                    <h2 className="age-restriction">{film.age_restriction}</h2>
                                    {userData ? 
                                    personalList && personalList.some(obj => obj.id === film.id) ?
                                    <button className="image-button" style={{border: "none"}} 
                                        onClick={(e)=>handleRemoveFromPersonalList(e, film.id)}>
                                        <img className="image-heart" src="/images/heart.png"/>
                                    </button> : 
                                    <button className="image-button" style={{border: "none"}} 
                                        onClick={(e)=>handleAddToPersonalList(e, film.id)}>
                                        <img className="image-hollow-heart" src="/images/hollow_heart.png" />
                                    </button>
                                    : ""}
                                </div>
                            </div>
                            <span className="film-description">{film.description}</span>
                            <div className="film-element-footer">
                                <h3 className="film-secondary-label">Publisher: {film.publisher}, Release year: {film.year}</h3>
                                <h3 className="film-secondary-label">{film.category}, {film.genre}</h3>
                            </div>
                        </div>
                    </div>
                ))}
                </div>
                <div className="page-buttons">
                    <button disabled={pageNumber === 0} className="page-button" onClick={()=>handlePageBack()}>{"<"}</button>
                    <div>
                        <button style={{marginRight: "15px"}} disabled={pageNumber === 0} className="page-button" onClick={()=>setPageNumber(0)}>{"<<"}</button>
                        <span>Page: {pageNumber+1} / {maxPageNumber+1}&nbsp;&nbsp;&nbsp;
                            Films on page: <select defaultValue={pageSize} className="page-size-dropdown" onChange={(e)=>{setPageSize(e.target.value)}}>
                            {pageSizeArray.map((size)=>{
                                return <option key={size}>{size}</option>;
                                })}
                        </select>
                        </span>
                        <button style={{marginLeft: "15px"}} disabled={pageNumber === maxPageNumber} className="page-button" onClick={()=>setPageNumber(maxPageNumber)}>{">>"}</button>
                    </div>
                    <button disabled={pageNumber === maxPageNumber} className="page-button" onClick={()=>handlePageForward()}>{">"}</button>
                </div>
            </div>
            <div className="filter-list">
                <h1 className="filter-header">Search Filters</h1>
                <div className="filter">
                    <label htmlFor="age-restriction">Age restriction:</label>
                    <select {...register("ageRestriction")} id="age-restriction" className="filter-input">
                        <option value="">Any age</option>
                        <option value="SIX_PLUS">6+</option>
                        <option value="TWELVE_PLUS">12+</option>
                        <option value="SIXTEEN_PLUS">16+</option>
                        <option value="EIGHTEEN_PLUS">18+</option>
                    </select>
                </div>
                <div className="filter">
                    <label htmlFor="genre">Genre:</label>
                    <select {...register("genre")} id="genre" className="filter-input" >
                        <option value="">Any genre</option>
                        <option value="ACTION_FILM">Action film</option>
                        <option value="COMEDY">Comedy</option>
                        <option value="DRAMA">Drama</option>
                    </select>
                </div>
                <div className="filter">
                    <label htmlFor="year">Release year:</label> 
                    <input {...register("year", {
                        validate: (value) => {
                            if (!isNaN(value) && !isNaN(parseInt(value)))
                            {
                                if (value > currentYear) {
                                return `Enter a number between 1900 and ${currentYear}`;
                                }
                                if (value < 1900) {
                                    return `Enter a number between 1900 and ${currentYear}`;
                                }
                            }
                            return true;
                        }
                    })} id="year" type="number" className="filter-input" placeholder="Any year" min="1900" max={currentYear} />
                    {errors.year && (<label htmlFor='year' style={{color: "#CC0000", marginTop: "15px", marginBottom: "0"}}>
                        {errors.year.message}</label>)}
                </div>
            </div>
        </div>
    </>;
}

export default Films;