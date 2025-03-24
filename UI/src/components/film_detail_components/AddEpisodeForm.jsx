import React, { useState } from "react";
import "../../pages/FilmDetail.css"

const AddEpisodeForm = ({ 
  userData, 
  register, 
  handleEpisodeCreate,
  errors, 
  showForm,
  setShowForm
}) => {

  return (
    <div
      style={{
        display: "flex",
        width: "100%",
        marginTop: "15px",
        justifyContent: "center",
      }}
    >
      {userData && userData.role === "ADMIN" && (
        <button
          className="add-episode-button"
          onClick={() => setShowForm((prev) => !prev)}
          style={{ marginRight: "10px", padding: "10px" }}
        >
          {showForm ? "Cancel" : "Add Episode"}
        </button>
      )}
      {showForm && (
        <form className="add-episode-form" onSubmit={handleEpisodeCreate}>
          <div style={{ marginBottom: "10px" }}>
            <label htmlFor="season_num">Season Number:</label>
            <input
              id="season_num"
              type="number"
              className="add-episode-input"
              data-test-cy="add-season-input"
              {...register("season_num", { 
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
            {errors.season_num && (
              <p style={{ color: "red", margin: "5px 0" }}>
                {errors.season_num.message}
              </p>
            )}
          </div>
          <div style={{ marginBottom: "10px" }}>
            <label htmlFor="episode_num">Episode Number:</label>
            <input
              id="episode_num"
              type="number"
              className="add-episode-input"
              data-test-cy="add-episode-input"
              {...register("episode_num", { 
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
            {errors.episode_num && (
              <p style={{ color: "red", margin: "5px 0" }}>
                {errors.episode_num.message}
              </p>
            )}
          </div>
          <button
            style={{ width: "100%", paddingTop: "5px", paddingBottom: "5px" }}
            className="add-episode-button"
            type="submit"
          >
            Add Episode
          </button>
        </form>
      )}
    </div>
  );
}

export default AddEpisodeForm;