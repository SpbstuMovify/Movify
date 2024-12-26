package com.polytech.contentservice.dto;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class TokenValidationDto {
  private boolean isValid;
}
