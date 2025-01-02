package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.castmember.CastMemberDto;
import com.polytech.contentservice.entity.CastMember;
import java.time.LocalDateTime;
import java.util.Set;
import java.util.stream.Collectors;
import org.springframework.stereotype.Component;

/**
 * Маппер для CastMember.
 */
@Component
public class CastMemberMapper {
  public CastMemberDto convertToCastMemberDto(CastMember castMember) {
    return CastMemberDto.builder()
        .id(castMember.getId())
        .roleName(castMember.getRole())
        .employeeFullName(castMember.getFullName())
        .build();
  }

  public Set<CastMemberDto> convertToSetOfCastMemberDto(Set<CastMember> contents) {
    return contents.stream()
        .map(this::convertToCastMemberDto)
        .collect(Collectors.toSet());
  }

  public CastMember convertToCastMemberEntity(CastMemberDto castMember) {
    return CastMember.builder()
        .role(castMember.roleName())
        .fullName(castMember.employeeFullName())
        .updateDate(LocalDateTime.now())
        .creationDate(LocalDateTime.now())
        .build();
  }

  public Set<CastMember> convertToSetOfCastMemberEntity(Set<CastMemberDto> contents) {
    return contents.stream()
        .map(this::convertToCastMemberEntity)
        .collect(Collectors.toSet());
  }
}
