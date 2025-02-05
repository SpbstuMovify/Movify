import React, { useEffect, useState } from "react";
import Navigation from "../components/general/Navigation";
import { removeFromPersonalList, getPersonalList } from "../services/api";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import FilmList from "../components/flim_components/FilmList";
import mappingJSON from "../configs/config";

function FavoriteFilms() {
  const { userData, clearUserData } = useAuth();
  const navigate = useNavigate();
  const [favoriteFilms, setFavoriteFilms] = useState([]);
  const [hoveredId, setHoveredId] = useState(null);

  const processFilms = (filmsResponse) => {
    const processed = filmsResponse.map((film) => {
      let spacedString = film.genre.replace(/_/g, " ");
      film.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
      spacedString = film.category.replace(/_/g, " ");
      film.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
      return {
        ...film,
        age_restriction: mappingJSON().age_restriction[film.age_restriction],
        publisher: film.publisher.replace(/_/g, " ")
      };
    });
    setFavoriteFilms(processed);
  };

  const getFilms = async () => {
    try {
      const filmsResponse = await getPersonalList(userData.user_id, userData.token);
      processFilms(filmsResponse);
    } catch (error) {
      if (error.response?.status === 401) {
        clearUserData();
        navigate("/login");
      } else {
        console.error(error.message);
      }
    }
  };

  useEffect(() => {
    if (userData) {
      getFilms();
    }
  }, [userData]);

  const handleRemoveFromPersonalList = async (filmId) => {
    try {
      await removeFromPersonalList(userData.user_id, filmId, userData.token);
      setFavoriteFilms(favoriteFilms.filter((film) => film.id !== filmId));
    } catch (error) {
      console.error(error.message);
    }
  };

  return (
    <>
      <Navigation />
      <div className="films-body">
        <div
          className="film-list"
          style={{
            boxShadow: "0 4px 10px rgba(0, 0, 0, 0.3)",
            border: "1px solid black",
            height: "fit-content"
          }}
        >
          <h1 style={{ textAlign: "center" }}>Your favorite films and series</h1>
          <FilmList
            films={favoriteFilms}
            hoveredId={hoveredId}
            onCardMouseEnter={setHoveredId}
            onCardMouseLeave={() => setHoveredId(null)}
            onCardClick={(id) => navigate(`/films/${id}`)}
            onRemoveFavorite={handleRemoveFromPersonalList}
            userData={userData}
            personalList={favoriteFilms}
          />
        </div>
      </div>
    </>
  );
}

export default FavoriteFilms;