import { useEffect, useState } from "react";
import Navigation from "../components/Navigation";
import './Films.css'
import { removeFromPersonalList, getPersonalList, addToPersonalList } from "../services/api";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

function FavoriteFilms(){
    const {userData} = useAuth();
    const navigate = useNavigate();
    const [favoriteFilms, setFavoriteFilms] = useState(null);

    const [hoveredId, setHoveredId] = useState(null);

    const mappingJSON = {
        "age_restriction": {
            "EIGHTEEN_PLUS": "18+",
            "SIXTEEN_PLUS": "16+",
            "TWELVE_PLUS": "12+",
            "SIX_PLUS": "6+",
            "ZERO_PLUS": "0+"
        }
    }

    const processFilms = (filmsResponse) => {
        filmsResponse.forEach((film)=>{
            film.age_restriction = mappingJSON.age_restriction[film.age_restriction]
            let spacedString = film.genre.replace(/_/g, " ");
            film.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            spacedString = film.category.replace(/_/g, " ");
            film.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            film.publisher = film.publisher.replace(/_/g, " ");
        });    
        setFavoriteFilms(filmsResponse);
    }

    const getFilms = async () => {
        try {
            const filmsResponse = await getPersonalList(userData.user_id, userData.token); 
            processFilms(filmsResponse);
        }
        catch (error) {
            switch (error.response?.status) {
                case 401:
                    clearUserData();
                    navigate('/login');
                    break;

                default:
                    console.error(error.message);
            }
        }
    }

    useEffect( () => {
        if (userData) {
            getFilms();
        }
    },[userData]);

     const handleAddToPersonalList = async (event, contentId) => {
            event.stopPropagation();
            try {
                const response = await addToPersonalList(userData.user_id, contentId, userData.token);
                setFavoriteFilms([...favoriteFilms, {id: response.content_id}])
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
            setFavoriteFilms(favoriteFilms.filter((item)=>item.id !== contentId));
        }
        catch (error)
        {
            console.error(error.message);
        }
    }

    return <>
        <Navigation/>
        <div className="films-body">
            <div className="film-list" style={{boxShadow: "0 4px 10px rgba(0, 0, 0, 0.3)", border: "1px black solid", height: "fit-content"}}>
                <h1 style={{textAlign: "center"}}>Your favorite films and series</h1>
                <div className="film-container">
                {favoriteFilms && favoriteFilms.length !== 0 ? favoriteFilms.map((film) => (
                    <div key={film.id} className={film.id == hoveredId ? "film-element film-element-hover" : "film-element"}       
                        onMouseEnter={()=>{setHoveredId(film.id)}}
                        onMouseLeave={()=>{setHoveredId(null)}}
                        onClick={()=>navigate(`/films/${film.id}`)}>
                        <img className="film-logo" src={film.thumbnail ? `http://localhost:8090${film.thumbnail}` : "/images/no_image.jpg"} />
                        <div className="film-info">
                            <div className="film-element-header">
                                <h2 className="film-title">{film.title}</h2>
                                <div style={{display: "flex", gap: "10px"}}>
                                    <h2 className="age-restriction">{film.age_restriction}</h2>
                                    {userData ? 
                                    favoriteFilms && favoriteFilms.some(obj => obj.id === film.id) ?
                                    <button className="image-button" style={{border: "none"}} 
                                        onMouseEnter={()=>{setHoveredId(null)}}
                                        onMouseLeave={()=>{setHoveredId(film.id)}}
                                        onClick={(e)=>handleRemoveFromPersonalList(e, film.id)}>
                                        <img className="image-heart" src="/images/heart.png"/>
                                    </button> : 
                                    <button className="image-button" style={{border: "none"}}  
                                        onMouseEnter={()=>{setHoveredId(null)}}
                                        onMouseLeave={()=>{setHoveredId(film.id)}}
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
                )) : <h2 style={{textAlign: "center"}}>You have no favorite films right now...</h2>}
                </div>
            </div>
        </div>
    </>;
}

export default FavoriteFilms;