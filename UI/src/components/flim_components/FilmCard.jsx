import React from "react";
import { apiBaseURL } from "../../services/api";
import "../../pages/Films.css"

const FilmCard = ({ film, isHovered, onMouseEnter, onDeleteFilm, onMouseLeave, onClick, onAddFavorite, onRemoveFavorite, userData, personalList }) => {
  const isFavorite = personalList && personalList.some(obj => obj.id === film.id);

  return (
    <div
      key={film.id}
      className={isHovered ? "film-element film-element-hover" : "film-element"}
      onMouseEnter={() => onMouseEnter(film.id)}
      onMouseLeave={() => onMouseLeave()}
      onClick={() => onClick(film.id)}
    >
      <img
        data-testid="film-logo"
        className="film-logo"
        src={film.thumbnail ? `${apiBaseURL}${film.thumbnail}` : "/images/no_image.jpg"}
      />
      <div className="film-info">
        <div className="film-element-header">
          <h2 className="film-title">{film.title}</h2>
          <div style={{ display: "flex", gap: "10px" }}>
            <h2 className="age-restriction">{film.age_restriction}</h2>
            {userData && (
              isFavorite ? (
                <button className="image-button" style={{ border: "none" }}
                  onMouseEnter={() => onMouseLeave()}
                  onMouseLeave={() => onMouseEnter(film.id)}
                  onClick={(e) => { e.stopPropagation(); onRemoveFavorite(film.id); }}>
                  <img className="image-heart" src="/images/heart.png" alt="Remove from favorites"/>
                </button>
              ) : (
                <button className="image-button" style={{ border: "none" }}
                  onMouseEnter={() => onMouseLeave()}
                  onMouseLeave={() => onMouseEnter(film.id)}
                  onClick={(e) => { e.stopPropagation(); onAddFavorite(film.id); }}>
                  <img className="image-hollow-heart" src="/images/hollow_heart.png" alt="Add to favorites"/>
                </button>
              )
            )}
          </div>
        </div>
        <span className="film-description">{film.description}</span>
        <div className="film-element-footer">
          <h3 className="film-secondary-label">
            Publisher: {film.publisher}, Release year: {film.year}
          </h3>
          <h3 className="film-secondary-label" style={{ display: "flex", alignItems: "flex-end" }}>
            {film.category}, {film.genre}
            {userData && userData.role === "ADMIN" && (
              <button className="film-button" style={{ marginLeft: "10px", width:"fit-content" }}
                onClick={(e) => { e.stopPropagation(); onDeleteFilm(film.id); }}>
                Delete
              </button>
            )}
          </h3>
        </div>
      </div>
    </div>
  );
}

export default FilmCard;