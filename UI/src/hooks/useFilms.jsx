import { useState, useEffect } from "react";
import { searchFilms } from "../services/api";
import mappingJSON from "../configs/config";

export function useFilms(queryParams, pageSizeInitial = 5, initialPageNumber = 0) {
  const [films, setFilms] = useState([]);
  const [pageSize, setPageSize] = useState(pageSizeInitial);
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const [maxPageNumber, setMaxPageNumber] = useState(100);

  const getPages = (filmsResponse) => {
    filmsResponse.content.forEach((film) => {
      film.age_restriction = mappingJSON().age_restriction[film.age_restriction];
      let spacedString = film.genre.replace(/_/g, " ");
      film.genre = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
      spacedString = film.category.replace(/_/g, " ");
      film.category = spacedString.charAt(0).toUpperCase() + spacedString.slice(1).toLowerCase();
      film.publisher = film.publisher.replace(/_/g, " ");
    });
    setMaxPageNumber(filmsResponse.totalPages === 0 ? filmsResponse.totalPages : filmsResponse.totalPages - 1);
    if (filmsResponse.totalPages > 0 && filmsResponse.totalPages - 1 < pageNumber) {
      setPageNumber(filmsResponse.totalPages - 1);
    }
    setFilms(filmsResponse.content);
  };

  const getFilms = async () => {
    const filmsResponse = await searchFilms(pageSize, pageNumber, queryParams.get("year"), queryParams.get("genre"), queryParams.get("title"), queryParams.get("age_restriction"));
    getPages(filmsResponse);
  };

  useEffect(() => {
    getFilms();
  }, [pageNumber, pageSize, queryParams]);

  return {
    films,
    pageSize,
    setPageSize,
    pageNumber,
    setPageNumber,
    maxPageNumber,
    refetchFilms: getFilms,
  };
}