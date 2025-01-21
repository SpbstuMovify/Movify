package com.polytech.contentservice.entity;

import com.polytech.contentservice.common.AgeRestriction;
import com.polytech.contentservice.common.Category;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.common.Quality;
import com.polytech.contentservice.common.converter.AgeRestrictionConverter;
import com.polytech.contentservice.common.converter.QualityConverter;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Convert;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.ToString;
import org.hibernate.annotations.UuidGenerator;

/**
 * Сущность content.
 */
@Entity
@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@EqualsAndHashCode
public class Content {
  @Id
  @UuidGenerator
  @GeneratedValue
  @Column(name = "content_id")
  private UUID id;

  @Column(nullable = false)
  private String title;

  @Column(nullable = false)
  @Convert(converter = QualityConverter.class)
  private Quality quality;

  @Column(nullable = false)
  @Enumerated(EnumType.STRING)
  private Genre genre;

  @Column(nullable = false)
  @Enumerated(EnumType.STRING)
  private Category category;

  @Column(name = "age_restriction")
  @Convert(converter = AgeRestrictionConverter.class)
  private AgeRestriction ageRestriction;

  private String description;

  private String thumbnail;
  private Integer year;

  private String publisher;

  @Column(name = "creation_date")
  private LocalDateTime creationDate;
  @Column(name = "updated_date")
  private LocalDateTime updatedDate;

  @OneToMany(mappedBy = "content", fetch = FetchType.LAZY, cascade = CascadeType.ALL)
  @EqualsAndHashCode.Exclude
  @ToString.Exclude
  private Set<Episode> episodes;

  @OneToMany(targetEntity = CastMember.class, fetch = FetchType.LAZY, cascade = CascadeType.ALL)
  @JoinColumn(name = "content_id", referencedColumnName = "content_id")
  @EqualsAndHashCode.Exclude
  @ToString.Exclude
  private Set<CastMember> castMembers;
  @OneToMany(mappedBy = "content", cascade = CascadeType.ALL)
  @EqualsAndHashCode.Exclude
  @ToString.Exclude
  private Set<PersonalList> personalList = new HashSet<>();
}
