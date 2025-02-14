import React, { useEffect, useState } from "react";
import Navigation from "../components/general/Navigation";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { useForm } from "react-hook-form";
import mappingJSON from "../configs/config";

import {
  getEpisodes,
  getFilmById,
  getPersonalList,
  addToPersonalList,
  removeFromPersonalList,
  updateFilm,
  createEpisode,
  updateEpisode,
  deleteEpisode,
  uploadImage,
  uploadVideo,
  apiBaseURL
} from "../services/api";

import FilmDetailHeader from "../components/film_detail_components/FilmDetailHeader";
import FilmInfoSection from "../components/film_detail_components/FilmInfoSection";
import FilmEpisodesPanel from "../components/film_detail_components/FilmEpisodesPanel";
import AddEpisodeForm from "../components/film_detail_components/AddEpisodeForm";

function FilmDetail() {
  const { userData, clearUserData } = useAuth();
  const { contentId } = useParams();

  const [filmInfo, setFilmInfo] = useState(null);
  const [filmEpisodes, setFilmEpisodes] = useState([]);
  const [seasonArray, setSeasonArray] = useState([]);
  const [selectedSeason, setSelectedSeason] = useState();
  const [selectedEpisode, setSelectedEpisode] = useState();
  const [seasonEpisodeArray, setSeasonEpisodeArray] = useState([]);
  const [personalList, setPersonalList] = useState([]);
  const [isFavoritesUpdating, setIsFavoritesUpdating] = useState(false);
  const [thumbnailSource, setThumbnailSource] = useState("/images/no_image.jpg");
  const [showAddEpisodeForm, setShowAddEpisodeForm] = useState(false);
  const currentYear = new Date().getFullYear();

  const filmForm = useForm({
    defaultValues: filmInfo,
    mode: "onChange",
    reValidateMode: "onChange",
  });
  const filmEpisodesForm = useForm({
    defaultValues: selectedEpisode,
    mode: "onChange",
    reValidateMode: "onChange",
  });

  const getFilm = async () => {
    const response = await getFilmById(contentId);
    let spacedString = response.genre.replace(/_/g, " ");
    response.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
    spacedString = response.category.replace(/_/g, " ");
    response.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
    response.publisher = response.publisher.replace(/_/g, " ");
    setFilmInfo(response);
    setThumbnailSource(response.thumbnail ? `${apiBaseURL}${response.thumbnail}` : "/images/no_image.jpg");
  };

  const getFilmEpisodes = async () => {
    const response = await getEpisodes(contentId);
    setFilmEpisodes(response);
    setSeasonArray(Array.from(new Set(response.map((ep) => ep.season_num))).sort());
  };

  useEffect(() => {
    if (contentId) {
      getFilm();
      getFilmEpisodes();
    }
  }, [contentId]);

  useEffect(() => {
    if (filmInfo) {
      filmForm.reset(filmInfo);
    }
  }, [filmInfo]);

  useEffect(() => {
    if (filmEpisodes) {
      const episodes = filmEpisodes
        .filter((ep) => Number(ep.season_num) === Number(selectedSeason))
        .sort((a, b) => a.episode_num - b.episode_num);
      setSeasonEpisodeArray(episodes);
      if (selectedEpisode) {
        let newEp = episodes.find((ep) => ep.episode_num === selectedEpisode.episode_num);
        if (!newEp) {
          newEp = episodes.find((ep) => ep.episode_num === selectedEpisode.episode_num - 1);
          if (!newEp) {
            setSelectedSeason(undefined);
          }
        }
        setSelectedEpisode(newEp);
      }
    }
  }, [selectedSeason, filmEpisodes]);

  useEffect(() => {
    if (selectedEpisode) {
      filmEpisodesForm.reset(selectedEpisode);
    }
  }, [selectedEpisode]);

  useEffect(() => {
    const getUserPersonalList = async () => {
      try {
        const response = await getPersonalList(userData.user_id, userData.token);
        setPersonalList(response);
      } catch (error) {
        if (error.response?.status === 401) {
          clearUserData();
        } else {
          console.error(error.message);
        }
      }
    };
    if (userData) {
      getUserPersonalList();
    }
  }, [userData]);

  const handleAddToPersonalList = async (id) => {
    try {
      if (!isFavoritesUpdating) {
        setIsFavoritesUpdating(true);
        const response = await addToPersonalList(userData.user_id, id, userData.token);
        setPersonalList([...personalList, { id: response.content_id }]);
        setIsFavoritesUpdating(false);
      }
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  const handleRemoveFromPersonalList = async (id) => {
    try {
      if (!isFavoritesUpdating) {
        setIsFavoritesUpdating(true);
        await removeFromPersonalList(userData.user_id, id, userData.token);
        setPersonalList(personalList.filter((item) => item.id !== id));
        setIsFavoritesUpdating(false);
      }
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  const updateField = async (field, newValue, error) => {
    if (!error) {
      let parsedValue = newValue;
      if (field === "category" || field === "genre") {
        parsedValue = parsedValue.replace(/_/g, " ")
          .toLowerCase()
          .replace(/^\w/, (c) => c.toUpperCase());
      } else if (field === "age_restriction") {
        parsedValue = mappingJSON()["age_restriction"][newValue];
      }
      try {
        setFilmInfo({ ...filmInfo, [field]: parsedValue });
        await updateFilm(filmInfo.id, { [field]: newValue }, userData.token);
      } catch (error) {
        if (error.response?.status === 401) {
          clearUserData();
        } else {
          console.error(error.message);
        }
      }
    } else {
      filmForm.reset(filmInfo);
    }
  };

  const handleThumbnailChange = async (file) => {
    try {
      await uploadImage(filmInfo.id, file, userData.token);
      setTimeout(getFilm, 2000);
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  const handleVideoUpload = async (episodeId, file) => {
    try {
      await uploadVideo(filmInfo.id, episodeId, file, userData.token);
      setTimeout(getFilm, 1000);
      getFilmEpisodes();
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  const updateEpisodeField = async (episodeId, field, newValue, error) => {
    if (!error) {
      try {
        setFilmEpisodes((prev) =>
          prev.map((ep) => (ep.id === episodeId ? { ...ep, [field]: newValue } : ep))
        );
        setSeasonEpisodeArray((prev) =>
          prev.map((ep) => (ep.id === episodeId ? { ...ep, [field]: newValue } : ep))
        );
        await updateEpisode(episodeId, { [field]: newValue }, userData.token);
      } catch (error) {
        if (error.response?.status === 401) {
          clearUserData();
        } else {
          console.error(error.message);
        }
      }
    } else {
      filmEpisodesForm.reset(filmEpisodes);
    }
  };

  const deleteEpisodeById = async (episodeId, error) => {
    if (!error) {
      try {
        await deleteEpisode(episodeId, userData.token);
        getFilmEpisodes();
      } catch (error) {
        if (error.response?.status === 401) {
          clearUserData();
        } else {
          console.error(error.message);
        }
      }
    } else {
      filmEpisodesForm.reset(filmEpisodes);
    }
  };

  const handleEpisodeCreate = async (data) => {
    try {
      await createEpisode(contentId, data.episode_num, data.season_num, "New Episode", "", userData.token);
      getFilmEpisodes();
      setShowAddEpisodeForm(false);
    } catch (error) {
      console.error("Error adding the episode", error);
    }
  };

  const processEpisodeStatus = (status) => {
    switch (status) {
      case "NOT_UPLOADED":
        return userData && userData.role === "ADMIN"
          ? "The video has not been uploaded yet..."
          : "Coming soon...";
      case "IN_PROGRESS":
        return userData && userData.role === "ADMIN"
          ? "The video is uploading..."
          : "The episode is uploading, check again in a short while!";
      case "ERROR":
        return userData && userData.role === "ADMIN"
          ? "There was an error uploading the video!"
          : "Coming soon...";
      default:
        return "Coming soon...";
    }
  };

  return (
    <>
      <Navigation />
      <div className="detail-body" role="detail-body">
        <div className="detail-wrapper">
          <div className="film-detail-header">
            <FilmDetailHeader
                filmInfo={filmInfo}
                thumbnailSource={thumbnailSource}
                userData={userData}
                personalList={personalList}
                onThumbnailChange={handleThumbnailChange}
                onAddToPersonalList={handleAddToPersonalList}
                onRemoveFromPersonalList={handleRemoveFromPersonalList}
            />
            <FilmInfoSection
                filmInfo={filmInfo}
                filmForm={filmForm}
                userData={userData}
                updateField={updateField}
                mappingJSON={mappingJSON}
                currentYear={currentYear}
            />
          </div>
          <AddEpisodeForm 
            userData={userData}
            register={filmEpisodesForm.register}
            handleEpisodeCreate={filmEpisodesForm.handleSubmit(handleEpisodeCreate)}
            setShowForm={setShowAddEpisodeForm}
            errors={filmEpisodesForm.formState.errors}
            showForm={showAddEpisodeForm}
          />
          <FilmEpisodesPanel
            filmEpisodes={filmEpisodes}
            seasonArray={seasonArray}
            selectedSeason={selectedSeason}
            setSelectedSeason={setSelectedSeason}
            seasonEpisodeArray={seasonEpisodeArray}
            selectedEpisode={selectedEpisode}
            setSelectedEpisode={setSelectedEpisode}
            filmEpisodesForm={filmEpisodesForm}
            processEpisodeStatus={processEpisodeStatus}
            handleVideoUpload={handleVideoUpload}
            updateEpisodeField={updateEpisodeField}
            deleteEpisodeById={deleteEpisodeById}
            userData={userData}
          />
        </div>
      </div>
    </>
  );
}

export default FilmDetail;