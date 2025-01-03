--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
alter table episode drop column thumbnail;
alter table episode add unique (episode_num, season_num, content_id);
-----
