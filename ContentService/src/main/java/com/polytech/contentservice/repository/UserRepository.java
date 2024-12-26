package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.User;
import java.util.Optional;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * Взаимодействие с таблицей user.
 */
public interface UserRepository extends JpaRepository<User, UUID> {
    Optional<User> findByLogin(String login);

    Optional<User> findByEmail(String email);
}
