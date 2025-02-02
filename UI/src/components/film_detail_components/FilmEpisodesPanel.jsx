import React from "react";
import ReactPlayer from "react-player";
import { Controller } from "react-hook-form";
import EditableField from "../general/EditableField";
import "../../pages/FilmDetail.css"

const FilmEpisodesPanel = ({
  seasonArray,
  selectedSeason,
  setSelectedSeason,
  seasonEpisodeArray,
  selectedEpisode,
  setSelectedEpisode,
  filmEpisodesForm,
  processEpisodeStatus,
  handleVideoUpload,
  updateEpisodeField,
  deleteEpisodeById,
  userData,
}) => {
  return (
    <div className="player-wrapper">
      <div style={{ minWidth: "44.4vw", minHeight: "12vh" }}>
        <select
          onChange={(e) => {
            setSelectedSeason(e.target.value);
            setSelectedEpisode(null);
          }}
          className="episode-select"
          value={selectedSeason || ""}
        >
          <option value="" disabled hidden>
            Select season
          </option>
          {seasonArray &&
            seasonArray.map((season) => (
              <option value={season} key={season}>
                Season {season}
              </option>
            ))}
        </select>
        {selectedSeason && (
          <select
            onChange={(e) =>
              setSelectedEpisode(
                seasonEpisodeArray.find((episode) => Number(episode.episode_num) === Number(e.target.value))
              )
            }
            className="episode-select"
            value={selectedEpisode ? selectedEpisode.episode_num : ""}
          >
            <option value="" disabled hidden>
              Select episode
            </option>
            {seasonEpisodeArray &&
              seasonEpisodeArray.map((episode) => (
                <option value={episode.episode_num} key={episode.id}>
                  {episode.episode_num}. {episode.title}
                </option>
              ))}
          </select>
            )}
            {selectedEpisode ? (
            selectedEpisode.status === "UPLOADED" ? (
                <ReactPlayer
                url={`http://localhost:8090${selectedEpisode.s3_bucket_name}`}
                controls
                playing={false}
                width="44.4vw"
                height="25vw"
                />
            ) : (
                <div className="player-placeholder">{processEpisodeStatus(selectedEpisode.status)}</div>
            )
            ) : (
            ""
            )}
      </div>
      <div>
        {selectedEpisode && (
          <div style={{ marginLeft: "20px", width: "15vw" }}>
            <h2>
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
                    disableEditButton={!userData || userData.role !== "ADMIN"}
                    inputType={"input"}
                  />
                )}
              />
            </h2>
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
                  disableEditButton={!userData || userData.role !== "ADMIN"}
                  inputType={"textarea"}
                />
              )}
            />
            {userData && userData.role === "ADMIN" && (
              <div style={{ marginTop: "15px" }}>
                <h4>
                  Upload Video:
                  <label htmlFor="episode-upload">
                    <img className="upload-image" src="/images/upload.png" alt="Upload" />
                  </label>
                </h4>
                <input
                  style={{ display: "none" }}
                  id="episode-upload"
                  type="file"
                  accept="video/*"
                  onChange={(e) => handleVideoUpload(selectedEpisode.id, e.target.files[0])}
                />
              </div>
            )}
            {userData && userData.role === "ADMIN" && (
              <button className="episode-delete-button" onClick={() => deleteEpisodeById(selectedEpisode.id)}>
                Delete
              </button>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default FilmEpisodesPanel;