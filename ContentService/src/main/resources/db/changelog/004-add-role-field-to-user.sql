--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table "user" add column role varchar(32) not null check ( role in ('ADMIN', 'USER')) DEFAULT 'USER';

-----

-- alter table "user" drop column password;
