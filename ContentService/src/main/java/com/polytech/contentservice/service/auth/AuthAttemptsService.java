package com.polytech.contentservice.service.auth;

/**
 * Описание взаимодействия с таблицей auth_attempt.
 */
public interface AuthAttemptsService {
  /**
   * Проверка, что лимит попыток на вход не превышен.
   *
   * @param ip - IP адресс, с которого пытаются войти в систему
   * @return true - лимит превышен
   */
  boolean isMaxLoginAttemptsReached(String ip);

  /**
   * Может ли пользователь совершить попытку входа.
   * @param ip - адрес пользователя
   */
  boolean isLoginBlocked(String ip);

  /**
   * Обнуляет количество попыток входа.
   * @param ip - адрес пользователя
   */
  void resetLoginAttemptsByIp(String ip);
}
