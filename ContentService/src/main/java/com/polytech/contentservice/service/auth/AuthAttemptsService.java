package com.polytech.contentservice.service.auth;

/**
 * Описание взаимодействия с таблицей auth_attempt.
 */
public interface AuthAttemptsService {
  /**
   * Проверка, что лимит попыток на вход не превышен.
   *
   * @param ip - IP адресс, с которого пытаются войти в систему
   */
  void checkLoginAttempts(String ip);
}
