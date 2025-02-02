import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useForm } from "react-hook-form";
import Navigation from "../components/general/Navigation";
import FilmList from "../components/flim_components/FilmList";
import SearchForm from "../components/flim_components/SearchForm";
import FilterPanel from "../components/flim_components/FilterPanel";
import Pagination from "../components/flim_components/Pagination";
import { useFilms } from "../hooks/useFilms";
import { useAuth } from "../contexts/AuthContext";
import { addToPersonalList, removeFromPersonalList, createFilm, deleteFilm, getPersonalList } from "../services/api";

function Films() {
  const pageSizeOptions = [3, 5, 10, 20, 50];
  const { register, handleSubmit, formState: { errors }, setValue, setError } = useForm();
  const { userData, clearUserData } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [queryParams, setQueryParams] = useState(new URLSearchParams(location.search));
  const [hoveredId, setHoveredId] = useState(null);
  const [personalList, setPersonalList] = useState(null);
  const [isFavoritesUpdating, setIsFavoritesUpdating] = useState(false);

  const { films, pageSize, setPageSize, pageNumber, setPageNumber, maxPageNumber, refetchFilms } = useFilms(queryParams);

  const getUserPersonalList = async () => {
    try {
      const personalListResponse = await getPersonalList(userData.user_id, userData.token);
      setPersonalList(personalListResponse);
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  useEffect(() => {
    if (userData) {
      getUserPersonalList();
    }
  }, [userData]);

  useEffect(() => {
    setValue("title", queryParams.get("title") || "");
    setValue("ageRestriction", queryParams.get("age_restriction") || "");
    setValue("genre", queryParams.get("genre") || "");
    setValue("year", queryParams.get("year") || "");
  }, [queryParams, setValue]);

  const handleSearch = async (data) => {
    try {
      const paramsJSON = {
        title: data.title || undefined,
        year: data.year || undefined,
        genre: data.genre || undefined,
        age_restriction: data.ageRestriction || undefined
      };
      const newParams = new URLSearchParams(paramsJSON);
      setQueryParams(newParams);
      setPageNumber(0);
      navigate(`/films?${newParams.toString()}`);
    } catch (error) {
      setError("root", "Server error has occurred...");
      console.error(error.message);
    }
  };

  const handleAddToPersonalList = async (contentId) => {
    try {
      if (!isFavoritesUpdating) {
        setIsFavoritesUpdating(true);
        const response = await addToPersonalList(userData.user_id, contentId, userData.token);
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

  const handleRemoveFromPersonalList = async (contentId) => {
    try {
      if (!isFavoritesUpdating) {
        setIsFavoritesUpdating(true);
        await removeFromPersonalList(userData.user_id, contentId, userData.token);
        setPersonalList(personalList.filter(item => item.id !== contentId));
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

  const handleAddNewFilm = async () => {
    try {
      await createFilm(userData.token);
      refetchFilms();
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  const handleDeleteFilm = async (contentId) => {
    try {
      await deleteFilm(userData.token, contentId);
      refetchFilms();
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
      } else {
        console.error(error.message);
      }
    }
  };

  return (
    <>
      <Navigation />
      <div className="films-body">
        <div className="film-list">
          <h1 style={{ textAlign: "center" }}>Watch all movies and series you want!</h1>
          <SearchForm register={register} handleSubmit={handleSubmit} onSubmit={handleSearch} errors={errors} />
          <FilmList
            films={films}
            hoveredId={hoveredId}
            onCardMouseEnter={setHoveredId}
            onCardMouseLeave={() => setHoveredId(null)}
            onCardClick={(id) => navigate(`/films/${id}`)}
            onAddFavorite={handleAddToPersonalList}
            onRemoveFavorite={handleRemoveFromPersonalList}
            onDeleteFilm={handleDeleteFilm}
            userData={userData}
            personalList={personalList}
          />
          <Pagination
            pageNumber={pageNumber}
            maxPageNumber={maxPageNumber}
            onPageBack={() => pageNumber > 0 && setPageNumber(pageNumber - 1)}
            onPageForward={() => pageNumber < maxPageNumber && setPageNumber(pageNumber + 1)}
            onJumpFirst={() => setPageNumber(0)}
            onJumpLast={() => setPageNumber(maxPageNumber)}
            pageSize={pageSize}
            onPageSizeChange={(e) => setPageSize(e.target.value)}
            pageSizeOptions={pageSizeOptions}
          />
        </div>
        <div style={{ display: "flex", flexDirection: "column", alignItems: "center", gap: "15px" }}>
          <FilterPanel register={register} errors={errors} currentYear={new Date().getFullYear()} />
          {userData && userData.role === "ADMIN" && (
            <div>
              <button className="film-button" onClick={handleAddNewFilm}>Add new film</button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}

export default Films;
