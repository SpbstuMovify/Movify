package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.CastMemberDto;
import com.polytech.contentservice.entity.CastMember;

public class CastMemberMapper {
    public static CastMemberDto convertToCastMemberDto(CastMember castMember) {
        return CastMemberDto.builder()
                .id(castMember.getId())
                .roleName(castMember.getRole())
                .employeeFullName(castMember.getFullName())
                .build();
    }

    private CastMemberMapper() {

    }
}
