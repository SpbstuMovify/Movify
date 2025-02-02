import React from "react";
import FilmCard from "./FilmCard";

const FilmList = ({ films, hoveredId, onCardMouseEnter, onCardMouseLeave, onCardClick, onAddFavorite, onRemoveFavorite, onDeleteFilm, userData, personalList }) => {
  if (!films || films.length === 0) {
    return <h2 style={{ textAlign: "center" }}>No films found...</h2>;
  }

  return (
    <div className="film-container">
      {films.map((film) => (
        <FilmCard
          key={film.id}
          film={film}
          isHovered={film.id === hoveredId}
          onMouseEnter={onCardMouseEnter}
          onMouseLeave={onCardMouseLeave}
          onClick={onCardClick}
          onAddFavorite={onAddFavorite}
          onRemoveFavorite={onRemoveFavorite}
          onDeleteFilm={onDeleteFilm}
          userData={userData}
          personalList={personalList}
        />
      ))}
    </div>
  );
}

export default FilmList;