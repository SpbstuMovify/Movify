package com.polytech.contentservice.common;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

/**
 * Различные разрешения для видео.
 */
@RequiredArgsConstructor
@Getter
public enum Quality {
  P144("144P"),
  P240("240P"),
  P360("360P"),
  P480("480P"),
  P7200("720P"),
  P1080("1080P"),
  P1440("1440P"),
  P2160("2160P");

  private final String quality;
}
