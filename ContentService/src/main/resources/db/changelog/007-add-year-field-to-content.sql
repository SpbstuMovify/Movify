--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table content add column year int;

-----

-- alter table content drop column year;
