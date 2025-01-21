import ReactPlayer from "react-player";
import './FilmDetail.css'
import Navigation from '../components/Navigation';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { getEpisodes, getFilmById, getPersonalList, addToPersonalList, removeFromPersonalList, updateFilm, uploadImage } from '../services/api';
import { useAuth } from "../contexts/AuthContext";
import { useForm, Controller } from "react-hook-form";

import EditableField from "../components/EditableField";
import UpdatableImage from "../components/UpdatableImage";

function FilmDetail() {
    const { userData, clearUserData } = useAuth()
    const { contentId } = useParams();
    const [filmInfo, setFilmInfo] = useState();
    const [filmEpisodes, setFilmEpisodes] = useState();
    const [seasonArray, setSeasonArray] = useState();
    const [selectedSeason, setSelectedSeason] = useState();
    const [selectedEpisode, setSelectedEpisode] = useState();
    const [seasonEpisodeArray, setSeasonEpisodeArray] = useState();

    const { control, reset, getValues } = useForm({
        defaultValues: filmInfo,
        mode: "onChange", // Validate on every change
        reValidateMode: "onChange", // Revalidate when inputs change
    });

    const [personalList, setPersonalList] = useState();
    const [isFavoritesUpdating, setIsFavoritesUpdating] = useState(false);
    const [thumbnailSource, setThumbnailSource] = useState("/images/no_image.jpg");

    const currentYear = new Date().getFullYear()
    const mappingJSON = {
        "age_restriction": {
            "EIGHTEEN_PLUS": "18+",
            "SIXTEEN_PLUS": "16+",
            "TWELVE_PLUS": "12+",
            "SIX_PLUS": "6+",
            "ZERO_PLUS": "0+"
        },
        "age_restriction_options": [
            { value: "EIGHTEEN_PLUS", label: "18+" },
            { value: "SIXTEEN_PLUS", label: "16+" },
            { value: "TWELVE_PLUS", label: "12+" },
            { value: "SIX_PLUS", label: "6+" },
            { value: "ZERO_PLUS", label: "0+" }
        ],
        "category_options": [
            { value: "MOVIE", label: "Movie" },
            { value: "ANIMATED_SERIES", label: "Animated series" },
            { value: "SERIES", label: "Series" },
            { value: "ANIMATED_FILM", label: "Animated film" },
        ],
        "genre_options": [
            { value: "MUSICAL", label: "Musical" },
            { value: "DRAMA", label: "Drama" },
            { value: "THRILLER", label: "Thriller" },
            { value: "HORROR_FILM", label: "Horror film" },
            { value: "ACTION_FILM", label: "Action film" },
            { value: "BLOCKBUSTER", label: "Blockbuster" },
            { value: "COMEDY", label: "Comedy" },
            { value: "DOCUMENTARY", label: "Documentary" },
            { value: "CARTOON", label: "Cartoon" },
            { value: "HISTORICAL_FILM", label: "Historical film" }
        ]
    }

    useEffect(() => {
        if (filmInfo) {
            reset(filmInfo); // Set form values once the backend data is available
        }
    }, [filmInfo, reset]);

    const getFilm = async () => {
            const response = await getFilmById(contentId);
            let spacedString = response.genre.replace(/_/g, " ");
            response.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            spacedString = response.category.replace(/_/g, " ");
            response.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
            response.publisher = response.publisher.replace(/_/g, " ");
            setFilmInfo(response);
            setThumbnailSource(response.thumbnail ? `http://localhost:8090${response.thumbnail}` : "/images/no_image.jpg");
            console.log(response);
        }

    useEffect(()=>{
        const getFilmEpisodes = async () => {
            const response = await getEpisodes(contentId);
            setFilmEpisodes(response);
            setSeasonArray(Array.from(new Set(response.map((episode) => episode.season_num))));
        }
        if (contentId)
        {
            getFilm();
            getFilmEpisodes();
        }
    },[contentId]);

    useEffect(()=>{
        if (filmEpisodes)
        {
            setSeasonEpisodeArray(filmEpisodes.filter(episode=>(Number(episode.season_num) === Number(selectedSeason))));
        }
    }, [selectedSeason]);

    useEffect(()=>{
        const getUserPersonalList = async () => {
            try {
                const personalListResponse = await getPersonalList(userData.user_id, userData.token);
                setPersonalList(personalListResponse);
            }
            catch (error) {
                switch (error.response?.status) {
                    case 401:
                        clearUserData();
                        break;
    
                    default:
                        console.error(error.message);
                }
            }
        }
        if (userData)
        {
            getUserPersonalList(userData.user_id, );
        }
    }, [userData])

    const processEpisodeStatus = (status) => {
        switch(status)
        {
            case "NOT_UPLOADED":
                if (userData && userData.role == "ADMIN")
                {
                    return "The video has not been uploaded yet..."
                }
                return "Coming soon...";
            
            case "IN_PROGRESS":
                if (userData && userData.role == "ADMIN")
                {
                    return "The video is uploading..."
                }
                return "The episode is uploading, check again in a short while!";
                
            case "ERROR":
                if (userData && userData.role == "ADMIN")
                {
                    return "There was an error uploading the video!"
                }
                return "Coming soon...";
            
            default:
                return "Coming soon...";
                    
        }
    }

    const handleAddToPersonalList = async (contentId) => {
            try {
                if (!isFavoritesUpdating)
                {
                    setIsFavoritesUpdating(true);
                    const response = await addToPersonalList(userData.user_id, contentId, userData.token);
                    setPersonalList([...personalList, {id: response.content_id}]);
                    setIsFavoritesUpdating(false);
                }
            }
            catch (error)
            {
                switch (error.response?.status) {
                    case 401:
                        clearUserData();
                        break;
    
                    default:
                        console.error(error.message);
                }
            }
        }
    
        const handleRemoveFromPersonalList = async (contentId) => {
            try {
                if (!isFavoritesUpdating)
                {
                    setIsFavoritesUpdating(true);
                    const response = await removeFromPersonalList(userData.user_id, contentId, userData.token);
                    setPersonalList(personalList.filter((item)=>item.id !== contentId));
                    setIsFavoritesUpdating(false);
                }
            }
            catch (error)
            {
                switch (error.response?.status) {
                    case 401:
                        clearUserData();
                        break;
    
                    default:
                        console.error(error.message);
                }
            }
        }

    const updateField = async (field, newValue, error) => { 
        if (!error)
        {
            let parsedValue = newValue;
            if (field == "category" || field == "genre")
            {
                parsedValue = parsedValue.replace(/_/g, " ")
                                         .toLowerCase()
                                         .replace(/^\w/, (c) => c.toUpperCase());;
            }
            else if (field == "age_restriction") {
                parsedValue = mappingJSON["age_restriction"][parsedValue];
            }
            try {
                setFilmInfo({...filmInfo, [field]: parsedValue });
                const response = await updateFilm(filmInfo.id, {[field]: newValue}, userData.token);             
            }
            catch (error) {
                switch (error.response?.status) {
                    case 401:
                        clearUserData();
                        break;

                    default:
                        console.error(error.message);
                }
            }
        }
        else
        {
            reset(filmInfo);
        }
    };

    const handleThumbnailChange = async (file) => {
        try{
            
            await uploadImage(filmInfo.id, file, userData.token);
            await setTimeout(getFilm, 1000);
        }
        catch (error)
        {
            switch (error.response?.status) {
                case 401:
                    clearUserData();
                    break;

                default:
                    console.error(error.message);
            }
        }
    }

    return <>
        <Navigation />
        <div className="detail-body">
            <div className='detail-wrapper'>
                { filmInfo &&
                <div className='film-detail-header'>
                    <div className="film-detail-logo-container">
                        {userData && userData.role == "ADMIN" ?
                            <UpdatableImage className="film-detail-logo-uploadable"
                            onImageUpload={handleThumbnailChange}
                            src={thumbnailSource}/>
                            : <img className="film-detail-logo"
                            src={thumbnailSource} />
                        }
                        {userData ? 
                            personalList && personalList.some(obj => obj.id === filmInfo.id) ?
                            <button className="favorites-button" 
                                onClick={(e)=>handleRemoveFromPersonalList(filmInfo.id)}>
                                Remove from favorites <img className="image-heart" src="/images/heart.png"/>
                            </button> : 
                            <button className="favorites-button" 
                                onClick={(e)=>handleAddToPersonalList(filmInfo.id)}>
                                Add to favorites <img className="image-hollow-heart" src="/images/hollow_heart.png" />
                            </button>
                            : ""}
                    </div>
                    <div className="film-detail-info">
                        <h1 className="film-detail-title">
                            <Controller
                                name="title"
                                control={control}
                                render={({ field }) => (
                                    <EditableField
                                        label=""
                                        value={field.value}
                                        onSave={(newValue) => updateField("title", newValue)}
                                        onChange={field.onChange}
                                        disableEditButton={!userData || !(userData.role === "ADMIN")}
                                    />
                                )}
                            />
                        </h1>
                        <Controller
                            name="genre"
                            control={control}
                            render={({ field, fieldState }) => (
                                <>
                                    <EditableField
                                        label="Genre:"
                                        value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
                                        onSave={(newValue) => updateField("genre", newValue, fieldState.error)}
                                        onChange={field.onChange}
                                        disableEditButton={!userData || !(userData.role === "ADMIN")}
                                        inputType="select"
                                        options={mappingJSON.genre_options}
                                        selectDisplayedValue={field.value}
                                    />
                                    {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                </>
                            )}
                        />
                        <Controller
                            name="category"
                            control={control}
                            render={({ field, fieldState }) => (
                                <>
                                <EditableField
                                    label="Category:"
                                    value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
                                    onSave={(newValue) => updateField("category", newValue, fieldState.error)}
                                    onChange={field.onChange}
                                    disableEditButton={!userData || !(userData.role === "ADMIN")}
                                    inputType="select"
                                    options={mappingJSON.category_options}
                                    selectDisplayedValue={field.value}
                                />
                                {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                </>
                            )}
                        />
                        <Controller
                            name="age_restriction"
                            control={control}
                            render={({ field, fieldState }) => (
                                <>
                                    <EditableField
                                        label="Age restriction:"
                                        value={field.value}
                                        onSave={(newValue) => 
                                            {
                                                updateField("age_restriction", newValue, fieldState.error);
                                            }}
                                        onChange={field.onChange}
                                        disableEditButton={!userData || !(userData.role === "ADMIN")}
                                        inputType="select"
                                        options={mappingJSON.age_restriction_options}
                                        selectDisplayedValue={mappingJSON["age_restriction"][field.value]}
                                    />
                                    {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                </>
                            )}
                        />
                        <Controller
                            name="publisher"
                            control={control}
                            render={({ field }) => (
                                <EditableField
                                    label="Publisher:"
                                    value={field.value}
                                    onSave={(newValue) => updateField("publisher", newValue)}
                                    onChange={field.onChange}
                                    disableEditButton={!userData || !(userData.role === "ADMIN")}
                                />
                            )}
                        />
                        <Controller
                            name="year"
                            control={control}
                            rules={{
                                validate: (value) =>
                                {
                                    if (isNaN(value)) {
                                        return "Please enter a valid number";
                                    }
                                    if (value < 1900 || value > currentYear)
                                    {
                                        return "Please enter a year between 1900 and " + currentYear;
                                    }
                                    return true;
                                },
                            }}
                            render={({ field, fieldState }) => (
                                <>
                                <EditableField
                                    label="Release year:"
                                    value={field.value}
                                    onSave={(newValue) => {
                                        updateField("year", newValue, fieldState.error)
                                    }}
                                    onChange={field.onChange}
                                    disableEditButton={!userData || !(userData.role === "ADMIN")}
                                    inputType="number"
                                    maxNumber={currentYear}
                                    minNumber={1900}
                                />
                                {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                </>
                            )}
                        />
                        <Controller
                            name="description"
                            control={control}
                            render={({ field }) => (
                                <EditableField
                                    label="Description:"
                                    value={field.value}
                                    onSave={(newValue) => updateField("description", newValue)}
                                    onChange={field.onChange}
                                    disableEditButton={!userData || !(userData.role === "ADMIN")}
                                    inputType={"textarea"}
                                />
                            )}
                        />
                        {filmInfo.cast_members.length !== 0 ? (
                            <div style={{ display: "flex" }}>
                                <b style={{ marginRight: "15px", whiteSpace: "nowrap" }}>Cast members:</b>
                                <div>
                                    {filmInfo.cast_members.map((member, index) => (
                                        <div key={index} style={{ marginBottom: "10px" }}>
                                            <Controller
                                                name={`cast_members.${index}.employee_full_name`}
                                                control={control}
                                                render={({ field }) => (
                                                    <EditableField
                                                        label=""
                                                        value={field.value}
                                                        onSave={(newValue) => {
                                                            const updatedCastMembers = getValues("cast_members");
                                                            updatedCastMembers[index] = { ...updatedCastMembers[index], employee_full_name: newValue };
                                                            updateField("cast_members", updatedCastMembers);
                                                        }}
                                                        onChange={field.onChange}
                                                        disableEditButton={!userData || !(userData.role === "ADMIN")}
                                                        isPTag={false}
                                                    />
                                                )}
                                            />
                                            <Controller
                                                name={`cast_members.${index}.role_name`}
                                                control={control}
                                                render={({ field }) => (
                                                    <EditableField
                                                        label=" - "
                                                        value={field.value}
                                                        onSave={(newValue) => {
                                                            const updatedCastMembers = getValues("cast_members");
                                                            updatedCastMembers[index] = { ...updatedCastMembers[index], role_name: newValue };
                                                            updateField("cast_members", updatedCastMembers);
                                                        }}
                                                        onChange={field.onChange}
                                                        disableEditButton={!userData || !(userData.role === "ADMIN")}
                                                        isPTag={false}
                                                    />
                                                )}
                                            />
                                        </div>
                                    ))}
                                </div>
                            </div>
                        ) : null}
                    </div>
                </div>
                }
                <div className="player-wrapper">
                    <div style={{minWidth: "44.4vw", minHeight: "12vh"}}>
                        <select onChange={(e)=>{
                             setSelectedSeason(e.target.value);
                             setSelectedEpisode(null);}
                             } className='episode-select'
                             value={selectedSeason || ""}>
                            <option value="" disabled hidden>Select season</option>
                            {seasonArray && seasonArray.map((season)=>(<option value={season} key={season}>Season {season}</option>))}
                        </select>
                        { selectedSeason && 
                        <select onChange={(e)=>{
                            setSelectedEpisode(seasonEpisodeArray.find(episode=>
                                Number(episode.episode_num) === Number(e.target.value)))}} 
                            className='episode-select'
                            value={selectedEpisode ? selectedEpisode.episode_num : ""}>
                            <option value="" disabled hidden>Select episode</option>
                            {seasonEpisodeArray && seasonEpisodeArray.map((episode)=>(
                                <option value={episode.episode_num} key={episode.id}>{episode.episode_num}. {episode.title}</option>))}
                            {console.log(selectedEpisode)}
                        </select>
                        }
                        {selectedEpisode ? 
                                selectedEpisode.status == "UPLOADED" ? 
                                    <ReactPlayer url={`http://localhost:8090${selectedEpisode.s3_bucket_name}`}
                                        controls
                                        playing={false}
                                        width="44.4vw"
                                        height="25vw" /> 
                                    : <div className="player-placeholder"> 
                                    {processEpisodeStatus(selectedEpisode.status)} 
                                    </div> 
                                : ""}
                    </div>
                    {selectedEpisode && 
                    <div style={{marginLeft: "20px", width: "15vw"}}>
                        <>
                            <h3>{selectedEpisode.episode_num}. {selectedEpisode.title}</h3>
                            <h4 style={{marginBottom: "5px"}}>Description:</h4>
                            {selectedEpisode.description}
                        </>   
                    </div>
                    }
                </div>                
            </div>
        </div>
    </>;
}

export default FilmDetail;