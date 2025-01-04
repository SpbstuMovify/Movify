package com.polytech.contentservice.common;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

/**
 * Различные жанры для фильмов и сериалов.
 */
@Getter
@RequiredArgsConstructor
public enum Genre {
  ACTION_FILM("Боевик"),
  BLOCKBUSTER("Блокбастер"),
  CARTOON("Мультфильм"),
  COMEDY("Комедия"),
  DOCUMENTARY("Документальный фильм"),
  HISTORICAL_FILM("Исторический фильм"),
  HORROR_FILM("Ужастик"),
  MUSICAL("Мюзикл"),
  DRAMA("Драма"),
  THRILLER("Триллер");

  private final String name;
}
