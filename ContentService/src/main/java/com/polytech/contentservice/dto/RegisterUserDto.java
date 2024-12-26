package com.polytech.contentservice.dto;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class RegisterUserDto {
  private String token;
  private String passwordHash;
  private String passwordSalt;
}
