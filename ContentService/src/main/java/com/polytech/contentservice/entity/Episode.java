package com.polytech.contentservice.entity;

import com.polytech.contentservice.common.EpisodeStatus;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.UuidGenerator;

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

  private String title;

  private String storyline;

  @Enumerated(EnumType.STRING)
  private EpisodeStatus status;

  @Column(name = "s3_bucket_name")
  private String s3BucketName;

  @Column(name = "episode_num")
  private Integer episodeNumber;

  @Column(name = "season_num")
  private Integer seasonNumber;

  @JoinColumn(name = "content_id")
  @ManyToOne(fetch = FetchType.LAZY, cascade = CascadeType.ALL)
  private Content content;
}
