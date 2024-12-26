package com.polytech.contentservice.entity;

import com.polytech.contentservice.common.AgeRestriction;
import com.polytech.contentservice.common.Category;
import com.polytech.contentservice.common.Genre;
import com.polytech.contentservice.common.Quality;
import com.polytech.contentservice.common.converter.AgeRestrictionConverter;
import com.polytech.contentservice.common.converter.QualityConverter;
import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.UuidGenerator;

import java.time.LocalDateTime;
import java.util.Set;
import java.util.UUID;

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

    @Column(name = "age_restriction", nullable = false)
    @Convert(converter = AgeRestrictionConverter.class)
    private AgeRestriction ageRestriction;

    private String description;

    private String thumbnail;

    private String publisher;

    @Column(name = "creation_date", nullable = false)
    private LocalDateTime creationDate;
    @Column(name = "updated_date", nullable = false)
    private LocalDateTime updatedDate;

    @OneToMany(mappedBy = "content", fetch = FetchType.LAZY)
    @EqualsAndHashCode.Exclude
    @ToString.Exclude
    private Set<Episode> episodes;

    @OneToMany(mappedBy = "content", fetch = FetchType.LAZY)
    @EqualsAndHashCode.Exclude
    @ToString.Exclude
    private Set<CastMember> castMembers;
}
