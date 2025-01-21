package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.AuthAttempts;
import java.util.Optional;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * Взаимодействие с таблицей auth_attempt.
 */
public interface AuthAttemptsRepository extends JpaRepository<AuthAttempts, UUID> {
  /**
   * Получение записи из таблицы auth_attempt по ip.
   * @param ip  - адресс пользователя
   * @return Полученная запись
   */
  Optional<AuthAttempts> findByIp(String ip);

  /**
   * Удаление записи в таблице auth_attempt по ip.
   * @param ip - адресс пользователя
   */
  void deleteByIp(String ip);
}
