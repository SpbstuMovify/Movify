--liquibase formatted sql

--changeset author:llav3ji2019 failOnError:true
create extension if not exists "uuid-ossp";

create table if not exists content
(
    content_id      uuid primary key   default gen_random_uuid(),
    title           varchar   not null,
    quality         varchar   not null,
    genre           varchar   not null,
    category        varchar   not null,
    age_restriction varchar   not null,
    description     varchar,
    thumbnail       varchar,
    publisher       varchar,
    text            tsvector,
    creation_date   timestamp not null default now(),
    updated_date    timestamp not null default now(),

    constraint valid_genre check (genre in
                                  ('ACTION_FILM', 'BLOCKBUSTER', 'CARTOON', 'COMEDY', 'DOCUMENTARY', 'HISTORICAL_FILM',
                                   'HORROR_FILM', 'MUSICAL', 'DRAMA', 'THRILLER')),
    constraint valid_category check (category in ('MOVIE', 'SERIES', 'ANIMATED_FILM', 'ANIMATED_SERIES')),
    constraint valid_quality check (quality in ('144P', '240P', '360P', '480P', '720P', '1080P', '1440P', '2160P')),
    constraint valid_age_restriction check (age_restriction in ('6+', '12+', '16+', '18+'))
);

create index idx_gin_content_text on content using gin (text);

create table if not exists cast_member
(
    cast_member_id uuid primary key   default gen_random_uuid(),
    full_name      varchar   not null,
    content_id     uuid      not null,
    role           varchar   not null,
    creation_date  timestamp not null default now(),
    updated_date   timestamp not null default now(),

    constraint cast_member_fk1 foreign key (content_id) references content (content_id)
);

create table if not exists "user"
(
    user_id       uuid primary key        default gen_random_uuid(),
    login         varchar unique not null,
    first_name    varchar,
    last_name     varchar,
    email         varchar unique not null,
    phone         varchar,
    birthday      date,
    creation_date timestamp      not null default now(),
    updated_date  timestamp      not null default now()
);

create table if not exists personal_list
(
    personal_list_id uuid primary key   default gen_random_uuid(),
    user_id          uuid,
    content_id       uuid,
    creation_date    timestamp not null default now(),
    updated_date     timestamp not null default now(),

    constraint personal_list_fk1 foreign key (user_id) references "user" (user_id),
    constraint personal_list_fk2 foreign key (content_id) references content (content_id)
);

create table if not exists episode
(
    episode_id     uuid primary key default gen_random_uuid(),
    episode_num    int     not null,
    season_num     int,
    title          varchar not null,
    thumbnail      varchar,
    storyline      varchar,
    s3_bucket_name varchar,
    content_id     uuid    not null,

    constraint episode_fk1 foreign key (content_id) references content (content_id)
);

-----

-- rollback drop table episode;
-- rollback drop table personal_list;
-- rollback drop table "user";
-- rollback drop table cast_member;
-- rollback drop table content;
-- rollback drop index idx_gin_content_text;
-- rollback drop extension "uuid-ossp";
