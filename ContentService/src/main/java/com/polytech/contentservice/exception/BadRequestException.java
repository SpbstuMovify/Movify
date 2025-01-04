package com.polytech.contentservice.exception;

/**
 * Ошибка неверных параметров запроса.
 */
public class BadRequestException extends MovifyException {
  public BadRequestException(String message) {
    super(message);
  }
}
