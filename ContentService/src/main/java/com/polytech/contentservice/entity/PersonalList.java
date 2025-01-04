package com.polytech.contentservice.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import java.time.LocalDateTime;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.UuidGenerator;

/**
 * Сущность personal_list.
 */
@Entity
@Table(name = "personal_list")
@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class PersonalList {
  @Id
  @GeneratedValue
  @UuidGenerator
  @Column(name = "personal_list_id")
  private UUID id;

  @ManyToOne
  @JoinColumn(name = "user_id")
  private User user;

  @ManyToOne
  @JoinColumn(name = "content_id")
  private Content content;

  @Column(name = "creation_date")
  private LocalDateTime creationDate;
}
