package com.polytech.contentservice.exception;

/**
 * Ошибка при входе.
 */
public class LoginException extends MovifyException {
  public LoginException(String message) {
    super(message);
  }
}
