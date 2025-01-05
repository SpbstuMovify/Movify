package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.PersonalList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * Описание доступа personal_list.
 */
public interface PersonalListRepository extends JpaRepository<PersonalList, UUID> {
  Optional<PersonalList> findByUserIdAndContentId(UUID userId, UUID contentId);
  void deleteByUserIdAndContentId(UUID userId, UUID contentId);
}
