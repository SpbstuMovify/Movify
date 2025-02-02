import React from "react";
import UpdatableImage from "../general/UpdatableImage";
import "../../pages/FilmDetail.css"

const FilmDetailHeader = ({
  filmInfo,
  thumbnailSource,
  userData,
  personalList,
  onThumbnailChange,
  onAddToPersonalList,
  onRemoveFromPersonalList,
}) => {
  return (
    <div className="film-detail-logo-container">
    {userData && userData.role === "ADMIN" ? (
        <UpdatableImage
        className="film-detail-logo-uploadable"
        onImageUpload={onThumbnailChange}
        src={thumbnailSource}
        />
    ) : (
        <img className="film-detail-logo" src={thumbnailSource} alt="Film thumbnail" />
    )}
    {userData &&
        (personalList && personalList.some((item) => item.id === filmInfo?.id) ? (
        <button
            className="favorites-button"
            onClick={() => onRemoveFromPersonalList(filmInfo.id)}
        >
            Remove from favorites{" "}
            <img className="image-heart" src="/images/heart.png" alt="Remove favorite" />
        </button>
        ) : (
        <button
            className="favorites-button"
            onClick={() => onAddToPersonalList(filmInfo.id)}
        >
            Add to favorites{" "}
            <img className="image-hollow-heart" src="/images/hollow_heart.png" alt="Add favorite" />
        </button>
        ))}
    </div>
  );
};

export default FilmDetailHeader;