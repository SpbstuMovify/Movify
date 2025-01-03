package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.PersonalList;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * Описание доступа personal_list.
 */
public interface PersonalListRepository extends JpaRepository<PersonalList, UUID> {
}
