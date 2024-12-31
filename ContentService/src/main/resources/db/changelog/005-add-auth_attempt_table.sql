--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
create table if not exists auth_attempt
(
    auth_attempt_id     uuid primary key,
    ip                  varchar unique not null,
    attempts_left       int default 3,
    next_attempts_time  timestamp
);

-----

-- drop table "auth_attempt";
