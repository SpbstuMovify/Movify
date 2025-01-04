package com.polytech.contentservice.exception;

/**
 * Ошибка не найдено.
 */
public class NotFoundException extends MovifyException {
  public NotFoundException(String message) {
    super(message);
  }
}
