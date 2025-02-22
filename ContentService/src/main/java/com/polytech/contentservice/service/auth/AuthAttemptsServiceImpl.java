package com.polytech.contentservice.service.auth;

import com.polytech.contentservice.entity.AuthAttempts;
import com.polytech.contentservice.repository.AuthAttemptsRepository;
import jakarta.transaction.Transactional;
import java.time.LocalDateTime;
import java.util.Optional;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link AuthAttemptsService}.
 */
@Service
@RequiredArgsConstructor
public class AuthAttemptsServiceImpl implements AuthAttemptsService {
  private final AuthAttemptsRepository authAttemptsRepository;

  @Value("${auth.ban-time-in-seconds}")
  private int banTimeInSeconds;
  @Value("${auth.default-attempts-amount}")
  private int defaultAttemptsAmount;

  @Override
  @Transactional
  public boolean isMaxLoginAttemptsReached(String ip) {
    Optional<AuthAttempts> optionalAuthAttempts = authAttemptsRepository.findByIp(ip);
    if (optionalAuthAttempts.isEmpty()) {
      authAttemptsRepository.save(createDefaultAuthAttemptsEntity(ip));
      return false;
    }

    AuthAttempts authAttempts = optionalAuthAttempts.get();
    int authAttemptsLeft = authAttempts.getAttemptsLeft();

    if (authAttemptsLeft == 0 && authAttempts.getNextAttemptsTime().isAfter(LocalDateTime.now())) {
      return true;
    }

    if (authAttemptsLeft == 0 && authAttempts.getNextAttemptsTime().isBefore(LocalDateTime.now())) {
      authAttempts.setAuthAttemptsId(authAttempts.getAuthAttemptsId());
      authAttempts.setNextAttemptsTime(null);
      authAttempts.setAttemptsLeft(defaultAttemptsAmount - 1);
      authAttemptsRepository.save(authAttempts);
      return false;
    }
    if (authAttemptsLeft == 1) {
      authAttempts.setNextAttemptsTime(LocalDateTime.now().plusSeconds(banTimeInSeconds));
    }
    authAttempts.setAttemptsLeft(authAttempts.getAttemptsLeft() - 1);
    authAttemptsRepository.save(authAttempts);
    return false;
  }

  @Override
  public boolean isLoginBlocked(String ip) {
    Optional<AuthAttempts> optionalAuthAttempts = authAttemptsRepository.findByIp(ip);
    if (optionalAuthAttempts.isEmpty()) {
      return false;
    }

    AuthAttempts authAttempts = optionalAuthAttempts.get();
    int authAttemptsLeft = authAttempts.getAttemptsLeft();
    LocalDateTime nextAttemptsTime = authAttempts.getNextAttemptsTime();
    return (authAttemptsLeft == 0 && nextAttemptsTime.isAfter(LocalDateTime.now()));
  }

  @Override
  public void resetLoginAttemptsByIp(String ip) {
    Optional<AuthAttempts> optionalAuthAttempts = authAttemptsRepository.findByIp(ip);
    if (optionalAuthAttempts.isEmpty()) {
      return;
    }
    authAttemptsRepository.deleteById(optionalAuthAttempts.get().getAuthAttemptsId());
  }

  private AuthAttempts createDefaultAuthAttemptsEntity(String ip) {
    return AuthAttempts.builder()
        .ip(ip)
        .attemptsLeft(defaultAttemptsAmount - 1)
        .build();
  }
}
