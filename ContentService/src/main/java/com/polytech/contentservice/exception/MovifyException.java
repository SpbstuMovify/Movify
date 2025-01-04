package com.polytech.contentservice.exception;

/**
 * Внутренняя ошибка сервера.
 */
public class MovifyException extends RuntimeException {
  public MovifyException(String message) {
    super(message);
  }
}
