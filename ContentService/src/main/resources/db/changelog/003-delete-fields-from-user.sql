--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table "user" drop column birthday;
alter table "user" drop column phone;

-----

-- alter table "user" add column phone varchar;
-- alter table "user" add column birthday date;
