--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table "user" add column password varchar(10000) not null;

-----

-- alter table "user" drop column password;
