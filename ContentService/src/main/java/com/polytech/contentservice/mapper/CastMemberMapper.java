package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.castmember.CastMemberDto;
import com.polytech.contentservice.entity.CastMember;
import java.time.LocalDateTime;
import java.util.Collections;
import java.util.Map;
import java.util.Set;
import java.util.UUID;
import java.util.function.Function;
import java.util.stream.Collectors;
import org.springframework.stereotype.Component;

/**
 * Маппер для CastMember.
 */
@Component
public class CastMemberMapper {
  public CastMember patchUpdate(CastMember oldCastMember, CastMember newCastMember) {
    return CastMember.builder()
        .id(oldCastMember.getId())
        .content(oldCastMember.getContent())
        .creationDate(oldCastMember.getCreationDate())
        .updateDate(LocalDateTime.now())
        .role(newCastMember.getRole() != null ? newCastMember.getRole() : oldCastMember.getRole())
        .fullName(newCastMember.getFullName() != null ? newCastMember.getFullName() : oldCastMember.getFullName())
        .build();
  }

  public Set<CastMember> patchUpdate(Set<CastMember> oldCastMember, Set<CastMember> newCastMember) {
    if (newCastMember.isEmpty()) {
      return oldCastMember;
    }
    Map<UUID, CastMember> oldCastMemberMap = oldCastMember.stream().collect(Collectors.toMap(CastMember::getId, Function.identity()));
    return newCastMember.stream()
        .map(castMember -> patchUpdate(oldCastMemberMap.get(castMember.getId()), castMember))
        .collect(Collectors.toSet());
  }

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
        .id(castMember.id())
        .role(castMember.roleName())
        .fullName(castMember.employeeFullName())
        .updateDate(LocalDateTime.now())
        .creationDate(LocalDateTime.now())
        .build();
  }

  public Set<CastMember> convertToSetOfCastMemberEntity(Set<CastMemberDto> contents) {
    if (contents == null ) {
      return Collections.emptySet();
    }
    return contents.stream()
        .map(this::convertToCastMemberEntity)
        .collect(Collectors.toSet());
  }
}
