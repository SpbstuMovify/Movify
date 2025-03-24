import React from "react";

const SearchForm = ({ register, onSubmit, errors }) => {
  return (
    <form onSubmit={onSubmit} className="search-bar-wrapper">
      <input {...register("title")} className="search-bar" type="text" placeholder="Search..." />
      <button data-test-cy="search-button" className="image-button" type="submit">
        <img src="/images/search_button.png" alt="Search" />
      </button>
      {errors.root && (
        <label style={{ color: "#CC0000", marginTop: "15px", marginBottom: "0" }}>
          {errors.root.message}
        </label>
      )}
    </form>
  );
}

export default SearchForm;