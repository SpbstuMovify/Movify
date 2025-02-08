package com.polytech.contentservice.service.auth;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import com.polytech.contentservice.entity.AuthAttempts;
import com.polytech.contentservice.repository.AuthAttemptsRepository;
import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class AuthAttemptsServiceImplTest {
  @Mock
  private AuthAttemptsRepository authAttemptsRepository;

  @InjectMocks
  private AuthAttemptsServiceImpl authAttemptsService;

  private final String testIp = "192.168.1.1";

  @Test
  void testIsMaxLoginAttemptsReached_FirstAttempt() {
    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.empty());

    boolean result = authAttemptsService.isMaxLoginAttemptsReached(testIp);

    assertFalse(result);
    verify(authAttemptsRepository, times(1)).save(any(AuthAttempts.class));
  }

  @Test
  void testIsMaxLoginAttemptsReached_AttemptsRemaining() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(2);

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isMaxLoginAttemptsReached(testIp);

    assertFalse(result);
    assertEquals(1, authAttempts.getAttemptsLeft());
    verify(authAttemptsRepository, times(1)).save(authAttempts);
  }

  @Test
  void testIsMaxLoginAttemptsReached_LastAttempt() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(1);

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isMaxLoginAttemptsReached(testIp);

    assertFalse(result);
    assertEquals(0, authAttempts.getAttemptsLeft());
    assertNotNull(authAttempts.getNextAttemptsTime());
    verify(authAttemptsRepository, times(1)).save(authAttempts);
  }

  @Test
  void testIsMaxLoginAttemptsReached_BlockActive() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(0);
    authAttempts.setNextAttemptsTime(LocalDateTime.now().plusSeconds(30));

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isMaxLoginAttemptsReached(testIp);

    assertTrue(result);
    verify(authAttemptsRepository, never()).save(any());
  }

  @Test
  void testIsMaxLoginAttemptsReached_BlockExpired() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(0);
    authAttempts.setNextAttemptsTime(LocalDateTime.now().minusSeconds(10));

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isMaxLoginAttemptsReached(testIp);

    assertFalse(result);
    verify(authAttemptsRepository, times(1)).save(authAttempts);
  }

  @Test
  void testIsLoginBlocked_NoAttemptsRecord() {
    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.empty());

    boolean result = authAttemptsService.isLoginBlocked(testIp);

    assertFalse(result);
  }

  @Test
  void testIsLoginBlocked_NotBlocked() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(1);

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isLoginBlocked(testIp);

    assertFalse(result);
  }

  @Test
  void testIsLoginBlocked_Blocked() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setIp(testIp);
    authAttempts.setAttemptsLeft(0);
    authAttempts.setNextAttemptsTime(LocalDateTime.now().plusSeconds(30));

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    boolean result = authAttemptsService.isLoginBlocked(testIp);

    assertTrue(result);
  }

  @Test
  void testResetLoginAttemptsByIp_NoRecord() {
    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.empty());

    authAttemptsService.resetLoginAttemptsByIp(testIp);

    verify(authAttemptsRepository, never()).deleteById(any());
  }

  @Test
  void testResetLoginAttemptsByIp_RecordExists() {
    AuthAttempts authAttempts = new AuthAttempts();
    authAttempts.setAuthAttemptsId(UUID.randomUUID());
    authAttempts.setIp(testIp);

    when(authAttemptsRepository.findByIp(testIp)).thenReturn(Optional.of(authAttempts));

    authAttemptsService.resetLoginAttemptsByIp(testIp);

    verify(authAttemptsRepository, times(1)).deleteById(authAttempts.getAuthAttemptsId());
  }
}
