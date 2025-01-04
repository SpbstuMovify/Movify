--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table episode add column status varchar(50);
-----
-- alter table episode drop column status;
