package com.polytech.contentservice.entity;

import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.UuidGenerator;

import java.time.LocalDateTime;
import java.util.UUID;

/**
 * Сущность episode.
 */
@Entity
@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class Episode {
  @Id
  @GeneratedValue
  @UuidGenerator
  @Column(name = "episode_id")
  private UUID id;

  @Column(nullable = false)
  private String title;

  private String thumbnail;

  private String storyline;

  @Column(name = "s3_bucket_name")
  private String s3BucketName;

  @Column(name = "episode_num")
  private Integer episodeNumber;

  @Column(name = "season_num")
  private Integer seasonNumber;

  @JoinColumn(name = "content_id", insertable = false, updatable = false)
  @ManyToOne(targetEntity = Content.class, fetch = FetchType.LAZY)
  private Content content;
}
