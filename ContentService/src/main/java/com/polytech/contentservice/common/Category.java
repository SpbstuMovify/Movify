package com.polytech.contentservice.common;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

/**
 * Различные категории фильмов
 */
@Getter
@RequiredArgsConstructor
public enum Category {
  MOVIE("Фильм"),
  SERIES("Сериал"),
  ANIMATED_FILM("Анимационный фильм"),
  ANIMATED_SERIES("Анимационный сериал");

  private final String name;
}
