package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.AuthAttempts;
import java.util.Optional;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * Взаимодействие с таблицей auth_attempt.
 */
public interface AuthAttemptsRepository extends JpaRepository<AuthAttempts, UUID> {
  Optional<AuthAttempts> findByIp(String ip);
}
