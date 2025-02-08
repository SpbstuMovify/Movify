package com.polytech.contentservice.mapper;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.dto.castmember.CastMemberDto;
import com.polytech.contentservice.entity.CastMember;
import java.time.LocalDateTime;
import java.util.Set;
import java.util.UUID;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Spy;
import org.junit.jupiter.api.Test;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class CastMemberMapperTest {
  @Spy
  private CastMemberMapper mapper;

  @Test
  void testPatchUpdateSingle() {
    CastMember oldMember = CastMember.builder()
        .id(UUID.randomUUID())
        .role("Actor")
        .fullName("John Doe")
        .creationDate(LocalDateTime.now().minusDays(1))
        .updateDate(LocalDateTime.now().minusDays(1))
        .build();

    CastMember newMember = CastMember.builder()
        .role("Director")
        .fullName("John Smith")
        .build();

    CastMember updatedMember = mapper.patchUpdate(oldMember, newMember);

    assertEquals(oldMember.getId(), updatedMember.getId());
    assertEquals("Director", updatedMember.getRole());
    assertEquals("John Smith", updatedMember.getFullName());
    assertNotEquals(oldMember.getUpdateDate(), updatedMember.getUpdateDate());
  }

  @Test
  void testPatchUpdateSet() {
    UUID id = UUID.randomUUID();
    CastMember oldMember = CastMember.builder().id(id).role("Actor").fullName("John Doe").build();
    CastMember newMember = CastMember.builder().id(id).role("Director").fullName("John Smith").build();

    Set<CastMember> updatedSet = mapper.patchUpdate(Set.of(oldMember), Set.of(newMember));

    assertEquals(1, updatedSet.size());
    CastMember updatedMember = updatedSet.iterator().next();
    assertEquals("Director", updatedMember.getRole());
    assertEquals("John Smith", updatedMember.getFullName());
  }

  @Test
  void testConvertToCastMemberDto() {
    UUID id = UUID.randomUUID();
    CastMember castMember = CastMember.builder().id(id).role("Actor").fullName("John Doe").build();

    CastMemberDto dto = mapper.convertToCastMemberDto(castMember);

    assertEquals(id, dto.id());
    assertEquals("Actor", dto.roleName());
    assertEquals("John Doe", dto.employeeFullName());
  }

  @Test
  void testConvertToSetOfCastMemberDto() {
    UUID id = UUID.randomUUID();
    CastMember castMember = CastMember.builder().id(id).role("Actor").fullName("John Doe").build();

    Set<CastMemberDto> dtos = mapper.convertToSetOfCastMemberDto(Set.of(castMember));

    assertEquals(1, dtos.size());
  }

  @Test
  void testConvertToCastMemberEntity() {
    UUID id = UUID.randomUUID();
    CastMemberDto dto = new CastMemberDto(id, "John Doe", "Actor");

    CastMember entity = mapper.convertToCastMemberEntity(dto);

    assertEquals(id, entity.getId());
    assertEquals("Actor", entity.getRole());
    assertEquals("John Doe", entity.getFullName());
  }

  @Test
  void testConvertToSetOfCastMemberEntity() {
    UUID id = UUID.randomUUID();
    CastMemberDto dto = new CastMemberDto(id, "John Doe", "Actor");

    Set<CastMember> entities = mapper.convertToSetOfCastMemberEntity(Set.of(dto));

    assertEquals(1, entities.size());
  }
}