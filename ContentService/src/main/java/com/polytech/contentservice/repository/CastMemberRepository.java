package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.CastMember;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

public interface CastMemberRepository extends JpaRepository<CastMember, UUID> {
}
