package com.polytech.contentservice.entity;

import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.UuidGenerator;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.UUID;

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
    private String password;
    private String email;
    private String phone;
    private LocalDate birthday;
    @Column(name = "creation_date")
    private LocalDateTime creationDate;
    @Column(name = "updated_date")
    private LocalDateTime updateDate;
}
