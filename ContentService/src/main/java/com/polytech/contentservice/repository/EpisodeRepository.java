package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.Episode;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

public interface EpisodeRepository extends JpaRepository<Episode, UUID> {
}
