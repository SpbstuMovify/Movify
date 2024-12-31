package com.polytech.contentservice.dto.castmember;

import com.fasterxml.jackson.annotation.JsonProperty;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Builder;

import java.util.UUID;

/**
 * Сущность члена состава
 *
 * @param id               Идентификатор сущности члена состава
 * @param employeeFullName Полное имя члена состава
 * @param roleName         Роль человека
 */
@Builder
@Schema(description = "Сущность члена состава")
public record CastMemberDto(
    @Schema(
        description = "Идентификатор сущности члена состава",
        example = "1a66d90d-f79e-46cf-b5fd-f759abae26e0"
    )
    UUID id,
    @Schema(description = "Полное имя члена состава", example = "Джонни Деп")
    @JsonProperty("employee_full_name")
    String employeeFullName,
    @Schema(description = "Роль", example = "Актёр")
    @JsonProperty("role_name")
    String roleName
) {
}

