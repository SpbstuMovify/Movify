--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table "user" add column password_hash varchar(10000) not null;
alter table "user" rename column password to password_salt;

-----

-- alter table "user" drop column password;
