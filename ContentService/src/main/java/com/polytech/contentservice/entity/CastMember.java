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
 * Сущность cast_member.
 */
@Entity
@Table(name = "cast_member")
@Getter
@Setter
@NoArgsConstructor
@Builder
@AllArgsConstructor
public class CastMember {
  @Id
  @UuidGenerator
  @GeneratedValue
  @Column(name = "cast_member_id")
  private UUID id;

  @JoinColumn(name = "content_id")
  @ManyToOne
  private Content content;

  @Column(nullable = false)
  private String role;

  @Column(name = "full_name")
  private String fullName;
  @Column(name = "creation_date")
  private LocalDateTime creationDate;
  @Column(name = "updated_date")
  private LocalDateTime updateDate;
}
