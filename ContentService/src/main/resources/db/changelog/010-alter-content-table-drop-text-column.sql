--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table content drop column text;
-----
