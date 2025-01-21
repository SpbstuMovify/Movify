import ReactPlayer from "react-player";
import './FilmDetail.css'
import Navigation from '../components/Navigation';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { getEpisodes, getFilmById, getPersonalList, addToPersonalList, removeFromPersonalList, updateFilm, createEpisode, updateEpisode, deleteEpisode, uploadImage, uploadVideo } from '../services/api';
import { useAuth } from "../contexts/AuthContext";
import { useForm, Controller } from "react-hook-form";
import mappingJSON from "../configs/config";

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
    const [showAddEpisodeForm, setShowAddEpisodeForm] = useState(false); // Управляет видимостью формы

    //const { episodeControl, episodeReset } = useForm({
    const filmEpisodesForm = useForm({
        defaultValues: selectedEpisode,
        mode: "onChange", // Validate on every change
        reValidateMode: "onChange", // Revalidate when inputs change
    })

    //const { control, reset, getValues } = useForm({
    const filmForm = useForm({
        defaultValues: filmInfo,
        mode: "onChange", // Validate on every change
        reValidateMode: "onChange", // Revalidate when inputs change
    });

    const [personalList, setPersonalList] = useState();
    const [isFavoritesUpdating, setIsFavoritesUpdating] = useState(false);
    const [thumbnailSource, setThumbnailSource] = useState("/images/no_image.jpg");

    const currentYear = new Date().getFullYear()

    useEffect(() => {
        if (filmInfo) {
            filmForm.reset(filmInfo); // Set form values once the backend data is available
        }
    }, [filmInfo, filmForm.reset]);

    const getFilm = async () => {
        const response = await getFilmById(contentId);
        let spacedString = response.genre.replace(/_/g, " ");
        response.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
        spacedString = response.category.replace(/_/g, " ");
        response.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
        response.publisher = response.publisher.replace(/_/g, " ");
        setFilmInfo(response);
        setThumbnailSource(response.thumbnail ? `http://localhost:8090${response.thumbnail}` : "/images/no_image.jpg");
    }
    const getFilmEpisodes = async () => {
        const response = await getEpisodes(contentId);
        setFilmEpisodes(response);
        setSeasonArray(Array.from(new Set(response.map((episode) => episode.season_num))).sort());
    }
    useEffect(() => {
        if (contentId) {
            getFilm();
            getFilmEpisodes();
        }
    }, [contentId]);

    useEffect(() => {
        if (filmEpisodes) {
            const episodes = filmEpisodes.filter(episode => 
                (Number(episode.season_num) === Number(selectedSeason))).sort((a,b)=>(a.episode_num-b.episode_num));
            setSeasonEpisodeArray(episodes);
            if (selectedEpisode)
            {
                let newEpisode = episodes.find(episode=>episode.episode_num == selectedEpisode.episode_num);
                if (!newEpisode) {
                    newEpisode = episodes.find(episode=>episode.episode_num == selectedEpisode.episode_num-1);
                    if (!newEpisode)
                    {
                        setSelectedSeason(undefined);
                    }
                }
                setSelectedEpisode(newEpisode);
            }
        }
    }, [selectedSeason, filmEpisodes]);

    useEffect(() => {
        if (selectedEpisode) {
            filmEpisodesForm.reset(selectedEpisode); 
        }
    }, [selectedEpisode, filmEpisodesForm]);

    useEffect(() => {
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
        if (userData) {
            getUserPersonalList(userData.user_id,);
        }
    }, [userData])

    const processEpisodeStatus = (status) => {
        switch (status) {
            case "NOT_UPLOADED":
                if (userData && userData.role == "ADMIN") {
                    return "The video has not been uploaded yet..."
                }
                return "Coming soon...";

            case "IN_PROGRESS":
                if (userData && userData.role == "ADMIN") {
                    return "The video is uploading..."
                }
                return "The episode is uploading, check again in a short while!";

            case "ERROR":
                if (userData && userData.role == "ADMIN") {
                    return "There was an error uploading the video!"
                }
                return "Coming soon...";

            default:
                return "Coming soon...";

        }
    }

    const handleAddToPersonalList = async (contentId) => {
        try {
            if (!isFavoritesUpdating) {
                setIsFavoritesUpdating(true);
                const response = await addToPersonalList(userData.user_id, contentId, userData.token);
                setPersonalList([...personalList, { id: response.content_id }]);
                setIsFavoritesUpdating(false);
            }
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

    const handleRemoveFromPersonalList = async (contentId) => {
        try {
            if (!isFavoritesUpdating) {
                setIsFavoritesUpdating(true);
                const response = await removeFromPersonalList(userData.user_id, contentId, userData.token);
                setPersonalList(personalList.filter((item) => item.id !== contentId));
                setIsFavoritesUpdating(false);
            }
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

    const updateField = async (field, newValue, error) => {
        if (!error) {
            let parsedValue = newValue;
            if (field == "category" || field == "genre") {
                parsedValue = parsedValue.replace(/_/g, " ")
                    .toLowerCase()
                    .replace(/^\w/, (c) => c.toUpperCase());;
            }
            else if (field == "age_restriction") {
                parsedValue = mappingJSON()["age_restriction"][parsedValue];
            }
            try {
                setFilmInfo({ ...filmInfo, [field]: parsedValue });
                await updateFilm(filmInfo.id, { [field]: newValue }, userData.token);
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
        else {
            filmForm.reset(filmInfo);
        }
    };

    const handleThumbnailChange = async (file) => {
        try {

            await uploadImage(filmInfo.id, file, userData.token);
            await setTimeout(getFilm, 2000);
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

    const handleVideoUpload = async (episodeId, file) => {
        try {
            await uploadVideo(filmInfo.id, episodeId, file, userData.token);
            await setTimeout(getFilm, 1000);
            getFilmEpisodes();
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

    const updateEpisodeField = async (episodeId, field, newValue, error) => {
        if (!error) {
            try {
                setFilmEpisodes((prev) =>
                    prev.map((episode) =>
                        episode.id === episodeId
                            ? { ...episode, [field]: newValue }
                            : episode
                    )
                );
                setSeasonEpisodeArray((prev) =>
                    prev.map((episode) =>
                        episode.id === episodeId
                            ? { ...episode, [field]: newValue }
                            : episode
                    )
                );
                await updateEpisode(episodeId, { [field]: newValue }, userData.token);
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
        else {
            filmEpisodesForm.episodeReset(filmEpisodes);
        }
    }

    const deleteEpisodeById = async (episodeId, error) => {
        if (!error) {
            try {
                await deleteEpisode(episodeId, userData.token);
                getFilmEpisodes();
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
        else {
            filmEpisodesForm.episodeReset(filmEpisodes);
        }
    }

    const handleEpisodeCreate = async (data) => {
        try {
            await createEpisode(contentId, data.episode_num, data.season_num, "New Episode", "", userData.token);
            getFilmEpisodes();
            setShowAddEpisodeForm(false); 
        } catch (error) {
            console.error("Error adding the episode", error);
        }
    };

    return <>
        <Navigation />
        <div className="detail-body">
            <div className='detail-wrapper'>
                {filmInfo &&
                    <div className='film-detail-header'>
                        <div className="film-detail-logo-container">
                            {userData && userData.role == "ADMIN" ?
                                <UpdatableImage className="film-detail-logo-uploadable"
                                    onImageUpload={handleThumbnailChange}
                                    src={thumbnailSource} />
                                : <img className="film-detail-logo"
                                    src={thumbnailSource} />
                            }
                            {userData ?
                                personalList && personalList.some(obj => obj.id === filmInfo.id) ?
                                    <button className="favorites-button"
                                        onClick={(e) => handleRemoveFromPersonalList(filmInfo.id)}>
                                        Remove from favorites <img className="image-heart" src="/images/heart.png" />
                                    </button> :
                                    <button className="favorites-button"
                                        onClick={(e) => handleAddToPersonalList(filmInfo.id)}>
                                        Add to favorites <img className="image-hollow-heart" src="/images/hollow_heart.png" />
                                    </button>
                                : ""}
                        </div>
                        <div className="film-detail-info">
                            <h1 className="film-detail-title">
                                <Controller
                                    name="title"
                                    control={filmForm.control}
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
                                control={filmForm.control}
                                render={({ field, fieldState }) => (
                                    <>
                                        <EditableField
                                            label="Genre:"
                                            value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
                                            onSave={(newValue) => updateField("genre", newValue, fieldState.error)}
                                            onChange={field.onChange}
                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                            inputType="select"
                                            options={mappingJSON().genre_options}
                                            selectDisplayedValue={field.value}
                                        />
                                        {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                    </>
                                )}
                            />
                            <Controller
                                name="category"
                                control={filmForm.control}
                                render={({ field, fieldState }) => (
                                    <>
                                        <EditableField
                                            label="Category:"
                                            value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
                                            onSave={(newValue) => updateField("category", newValue, fieldState.error)}
                                            onChange={field.onChange}
                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                            inputType="select"
                                            options={mappingJSON().category_options}
                                            selectDisplayedValue={field.value}
                                        />
                                        {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                    </>
                                )}
                            />
                            <Controller
                                name="age_restriction"
                                control={filmForm.control}
                                render={({ field, fieldState }) => (
                                    <>
                                        <EditableField
                                            label="Age restriction:"
                                            value={field.value}
                                            onSave={(newValue) => {
                                                updateField("age_restriction", newValue, fieldState.error);
                                            }}
                                            onChange={field.onChange}
                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                            inputType="select"
                                            options={mappingJSON().age_restriction_options}
                                            selectDisplayedValue={mappingJSON()["age_restriction"][field.value]}
                                        />
                                        {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
                                    </>
                                )}
                            />
                            <Controller
                                name="publisher"
                                control={filmForm.control}
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
                                control={filmForm.control}
                                rules={{
                                    validate: (value) => {
                                        if (isNaN(value)) {
                                            return "Please enter a valid number";
                                        }
                                        if (value < 1900 || value > currentYear) {
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
                                control={filmForm.control}
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
                                                {userData && userData.role == "ADMIN" && filmInfo.cast_members.length != 1 && <button onClick={()=>
                                                    updateField("cast_members", filmInfo.cast_members.filter((member_old)=>member !== member_old))}>-</button>}
                                                <Controller
                                                    name={`cast_members.${index}.employee_full_name`}
                                                    control={filmForm.control}
                                                    render={({ field }) => (
                                                        <EditableField
                                                            label=""
                                                            value={field.value}
                                                            onSave={(newValue) => {
                                                                const updatedCastMembers = filmForm.getValues("cast_members");
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
                                                    control={filmForm.control}
                                                    render={({ field }) => (
                                                        <EditableField
                                                            label=" - "
                                                            value={field.value}
                                                            onSave={(newValue) => {
                                                                const updatedCastMembers = filmForm.getValues("cast_members");
                                                                updatedCastMembers[index] = { ...updatedCastMembers[index], role_name: newValue };
                                                                updateField("cast_members", updatedCastMembers);
                                                            }}
                                                            onChange={field.onChange}
                                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                                            isPTag={false}
                                                        />
                                                    )}
                                                />
                                                {userData && userData.role == "ADMIN" && 
                                                    filmInfo.cast_members[filmInfo.cast_members.length-1] === member && <button onClick={()=>
                                                    updateField("cast_members", [...filmInfo.cast_members, {"employee_full_name": "Actor",
                                                    "role_name": "Role"}])}>+</button>}
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            ) : null}
                        </div>
                    </div>
                }
                <div>
                    {userData && userData.role === "ADMIN" && (
                        <button className="add-episode-button" onClick={() => setShowAddEpisodeForm(!showAddEpisodeForm)}>
                            {showAddEpisodeForm ? "Cancel" : "Add Episode"}
                        </button>
                    )}
                    {showAddEpisodeForm && (
                        <form className="add-episode-form" onSubmit={filmEpisodesForm.handleSubmit(handleEpisodeCreate)}>
                            <div>
                                <label htmlFor="season_num">Season Number:</label>
                                <input
                                    id="season_num"
                                    type="number"
                                    {...filmEpisodesForm.register("season_num", { 
                                        required: "Season number is required", 
                                        max: {
                                            value: 255,
                                            message: "Max season number is 255"
                                        },
                                        min: {
                                            value: 1,
                                            message: "Min season number is 1"
                                        } 
                                })}
                                />
                                {filmEpisodesForm.formState.errors.season_num && <p style={{ color: 'red' }}>{filmEpisodesForm.formState.errors.season_num.message}</p>}
                            </div>
                            <div>
                                <label htmlFor="episode_num">Episode Number:</label>
                                <input
                                    id="episode_num"
                                    type="number"
                                    {...filmEpisodesForm.register("episode_num", { 
                                        required: "Episode number is required", 
                                        max: {
                                            value: 9999,
                                            message: "Max episode number is 9999"
                                        },
                                        min: {
                                            value: 1,
                                            message: "Min episode number is 1"
                                        } 
                                })}
                                />
                                {filmEpisodesForm.formState.errors.episode_num && <p style={{ color: 'red' }}>{filmEpisodesForm.formState.errors.episode_num.message}</p>}
                            </div>
                            <button type="submit">Add Episode</button>
                        </form>
                    )}
                </div>
                <div className="player-wrapper">
                    <div style={{ minWidth: "44.4vw", minHeight: "12vh" }}>
                        <select onChange={(e) => {
                            setSelectedSeason(e.target.value);
                            setSelectedEpisode(null);
                        }
                        } className='episode-select'
                            value={selectedSeason || ""}>
                            <option value="" disabled hidden>Select season</option>
                            {seasonArray && seasonArray.map((season) => (<option value={season} key={season}>Season {season}</option>))}
                        </select>
                        {selectedSeason &&
                            <select onChange={(e) => {
                                setSelectedEpisode(seasonEpisodeArray.find(episode =>
                                    Number(episode.episode_num) === Number(e.target.value)))
                            }}
                                className='episode-select'
                                value={selectedEpisode ? selectedEpisode.episode_num : ""}>
                                <option value="" disabled hidden>Select episode</option>
                                {seasonEpisodeArray && seasonEpisodeArray.map((episode) => (
                                    <option value={episode.episode_num} key={episode.id}>{episode.episode_num}. {episode.title}</option>))}
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
                        <div style={{ marginLeft: "20px", width: "15vw" }}>
                            <>
                                <Controller
                                    name="title"
                                    control={filmEpisodesForm.control}
                                    defaultValue={selectedEpisode.title}
                                    render={({ field }) => (
                                        <EditableField
                                            label=""
                                            value={field.value}
                                            onSave={(newValue) => updateEpisodeField(selectedEpisode.id, "title", newValue)}
                                            onChange={field.onChange}
                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                            inputType={"input"}
                                        />
                                    )}
                                />
                                <Controller
                                    name="description"
                                    control={filmEpisodesForm.control}
                                    defaultValue={selectedEpisode.description}
                                    render={({ field }) => (
                                        <EditableField
                                            label="Description:"
                                            value={field.value}
                                            onSave={(newValue) => updateEpisodeField(selectedEpisode.id, "description", newValue)}
                                            onChange={field.onChange}
                                            disableEditButton={!userData || !(userData.role === "ADMIN")}
                                            inputType={"textarea"}
                                        />
                                    )}
                                />
                                {userData && userData.role === "ADMIN" && (
                                    <div style={{ marginTop: "15px" }}>
                                        <h4>Upload Video:</h4>
                                        <input
                                            type="file"
                                            accept="video/*"
                                            onChange={(e) => handleVideoUpload(selectedEpisode.id, e.target.files[0])}
                                        />
                                    </div>
                                )}
                                {userData && userData.role === "ADMIN" && (
                                    <button
                                        style={{
                                            marginTop: "15px",
                                            padding: "10px",
                                            backgroundColor: "red",
                                            color: "white",
                                            border: "none",
                                            borderRadius: "5px",
                                            cursor: "pointer",
                                        }}
                                        onClick={() => deleteEpisodeById(selectedEpisode.id)}
                                    >
                                        Delete
                                    </button>
                                )}
                            </>
                        </div>
                    }
                </div>
            </div>
        </div>
    </>;
}

export default FilmDetail;