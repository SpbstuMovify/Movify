package com.polytech.contentservice.entity;

import jakarta.persistence.*;
import lombok.*;

import java.time.LocalDateTime;
import java.util.UUID;

@Entity
@Table(name = "cast_member")
@Getter
@Setter
@NoArgsConstructor
@Builder
@AllArgsConstructor
public class CastMember {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "cast_member_id")
    private UUID id;

    @JoinColumn(name = "content_id", insertable = false, updatable = false)
    @ManyToOne(targetEntity = Content.class, fetch = FetchType.LAZY)
    private Content content;

    @Column(nullable = false)
    private String role;

    @Column(name = "full_name", nullable = false)
    private String fullName;
    @Column(name = "creation_date")
    private LocalDateTime creationDate;
    @Column(name = "updated_date")
    private LocalDateTime updateDate;
}
