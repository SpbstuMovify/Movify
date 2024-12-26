package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.Content;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.UUID;

/**
 * Взаимодействие с таблицей content.
 */
public interface ContentRepository extends JpaRepository<Content, UUID> {

}
