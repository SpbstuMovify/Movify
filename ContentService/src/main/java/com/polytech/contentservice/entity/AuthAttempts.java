package com.polytech.contentservice.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import java.time.LocalDateTime;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.UuidGenerator;

@Entity
@Table(name = "auth_attempt")
@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class AuthAttempts {
  @Id
  @UuidGenerator
  @GeneratedValue
  @Column(name = "auth_attempt_id")
  private UUID authAttemptsId;
  private String ip;
  @Column(name = "attempts_left")
  private Integer attemptsLeft;
  @Column(name = "next_attempts_time")
  private LocalDateTime nextAttemptsTime;
}
