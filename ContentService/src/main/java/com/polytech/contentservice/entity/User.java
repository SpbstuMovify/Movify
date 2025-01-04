package com.polytech.contentservice.entity;

import com.polytech.contentservice.common.Role;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.UuidGenerator;

/**
 * Сущность user.
 */
@Entity
@AllArgsConstructor
@NoArgsConstructor
@Builder
@Getter
@Setter
@Table(name = "`user`")
public class User {
  @Id
  @GeneratedValue
  @UuidGenerator
  @Column(name = "user_id")
  private UUID id;
  private String login;
  @Column(name = "first_name")
  private String firstName;
  @Column(name = "last_name")
  private String lastName;
  @Column(name = "password_hash")
  private String passwordHash;
  @Column(name = "password_salt")
  private String passwordSalt;
  @Enumerated(EnumType.STRING)
  private Role role;
  private String email;
  @Column(name = "creation_date")
  private LocalDateTime creationDate;
  @Column(name = "updated_date")
  private LocalDateTime updateDate;
  @OneToMany(mappedBy = "user", cascade = CascadeType.ALL)
  private Set<PersonalList> personalList = new HashSet<>();
}
