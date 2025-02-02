import React from "react";
import mappingJSON from "../../configs/config";

const FilterPanel = ({ register, errors, currentYear }) => {
  return (
    <div className="filter-list">
      <h1 className="filter-header">Search Filters</h1>
      <div className="filter">
        <label htmlFor="age-restriction">Age restriction:</label>
        <select {...register("ageRestriction")} id="age-restriction" className="filter-input">
          <option value="">Any age</option>
          {mappingJSON().age_restriction_options.map((option, index) => (
            <option value={option.value} key={index}>
              {option.label}
            </option>
          ))}
        </select>
      </div>
      <div className="filter">
        <label htmlFor="genre">Genre:</label>
        <select {...register("genre")} id="genre" className="filter-input">
          <option value="">Any genre</option>
          {mappingJSON().genre_options.map((option, index) => (
            <option value={option.value} key={index}>
              {option.label}
            </option>
          ))}
        </select>
      </div>
      <div className="filter">
        <label htmlFor="year">Release year:</label>
        <input
          {...register("year", {
            validate: (value) => {
              if (!isNaN(value) && !isNaN(parseInt(value))) {
                if (value > currentYear) {
                  return `Enter a number between 1900 and ${currentYear}`;
                }
                if (value < 1900) {
                  return `Enter a number between 1900 and ${currentYear}`;
                }
              }
              return true;
            }
          })}
          id="year"
          type="number"
          className="filter-input"
          placeholder="Any year"
          min="1900"
          max={currentYear}
        />
        {errors.year && (
          <label htmlFor="year" style={{ color: "#CC0000", marginTop: "15px", marginBottom: "0" }}>
            {errors.year.message}
          </label>
        )}
      </div>
    </div>
  );
};

export default FilterPanel;