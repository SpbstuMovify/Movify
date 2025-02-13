import '@testing-library/jest-dom'

export const register = jest.fn(()=>Promise.resolve({
    "login": "login",
    "email": "email@email.em",
    "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
    "role": "USER",
    "token": "dummy-token"
}));

export const login = jest.fn(()=>Promise.resolve({
    "login": "login",
    "email": "email@email.em",
    "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
    "role": "USER",
    "token": "dummy-token"
}));

export const getUserById = jest.fn(()=>Promise.resolve({
    "login": "login",
    "email": "email@email.em",
    "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
    "first_name": "first-name",
    "last_name": "last-name",
    "password": null,
    "role": "USER",
    "token": null,
    "search_type": null
}));

export const getUserByLogin = jest.fn(()=>Promise.resolve({
    "login": "login",
    "email": "email@email.em",
    "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
    "first_name": "first-name",
    "last_name": "last-name",
    "password": null,
    "role": "USER",
    "token": null,
    "search_type": null
}));

export const getUserByEmail = jest.fn(()=>Promise.resolve({
    "login": "login",
    "email": "email@email.em",
    "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
    "first_name": "first-name",
    "last_name": "last-name",
    "password": null,
    "role": "USER",
    "token": null,
    "search_type": null
}));

export const getFilmById = jest.fn(()=>Promise.resolve({
    "id": "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
    "year": 2024,
    "title": "Фоллаут",
    "quality": "P1080",
    "genre": "DRAMA",
    "category": "SERIES",
    "description": "Постапокалиптический мир после ядерной катастрофы. Герои борются за выживание в пустошах, где радиация, мутанты и жажда власти становятся главными угрозами.",
    "thumbnail": null,
    "publisher": "Amazon Prime Video",
    "age_restriction": "SIXTEEN_PLUS",
    "cast_members": [
        {
            "id": "d97f0fc7-a8d8-4417-8837-1440b1aa3802",
            "employee_full_name": "Кайл МакЛахлен",
            "role_name": "Ученый из убежища"
        },
        {
            "id": "d0445d71-4c2e-4815-83ba-44a1bb3e780d",
            "employee_full_name": "Уолтон Гоггинс",
            "role_name": "Мутант-одиночка"
        },
        {
            "id": "ada48311-18fe-410a-bd56-115f28f452cb",
            "employee_full_name": "Ксения Раппопорт",
            "role_name": "Лидер сопротивления"
        },
        {
            "id": "14449075-86df-4976-b3ea-77b2d89b69f6",
            "employee_full_name": "Элла Пернелл",
            "role_name": "Выжившая"
        }
    ]
}));

export const searchFilmsResponse = {
    "content": [
        {
            "id": "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
            "year": 2024,
            "title": "Фоллаут",
            "quality": "P1080",
            "genre": "DRAMA",
            "category": "SERIES",
            "description": "Постапокалиптический мир после ядерной катастрофы. Герои борются за выживание в пустошах, где радиация, мутанты и жажда власти становятся главными угрозами.",
            "thumbnail": null,
            "publisher": "Amazon Prime Video",
            "age_restriction": "SIXTEEN_PLUS",
            "cast_members": [
                {
                    "id": "d0445d71-4c2e-4815-83ba-44a1bb3e780d",
                    "employee_full_name": "Уолтон Гоггинс",
                    "role_name": "Мутант-одиночка"
                },
                {
                    "id": "d97f0fc7-a8d8-4417-8837-1440b1aa3802",
                    "employee_full_name": "Кайл МакЛахлен",
                    "role_name": "Ученый из убежища"
                },
                {
                    "id": "ada48311-18fe-410a-bd56-115f28f452cb",
                    "employee_full_name": "Ксения Раппопорт",
                    "role_name": "Лидер сопротивления"
                },
                {
                    "id": "14449075-86df-4976-b3ea-77b2d89b69f6",
                    "employee_full_name": "Элла Пернелл",
                    "role_name": "Выжившая"
                }
            ]
        }],
        "last": true,
        "totalPages": 1,
        "totalElements": 12,
        "size": 50,
        "number": 0,
        "sort": {
            "sorted": false,
            "empty": true,
            "unsorted": true
        },
        "first": true,
        "numberOfElements": 12,
        "empty": false
}

export const searchFilms = jest.fn(()=>Promise.resolve(searchFilmsResponse));

export const createFilm = jest.fn(()=>Promise.resolve({
    "id": "1b12236a-aca9-47bc-95ac-f3978836de2c",
    "year": 2000,
    "title": "Баки Ханма",
    "quality": "P1080",
    "genre": "ACTION_FILM",
    "category": "ANIMATED_SERIES",
    "description": "Баки Ханма интенсивно тренируется,чтобы превзойти отца, который считается сильнейшим бойцом в мире. В это же время пятеро самых жестоких заключенных-смертников совершают побег. Ими движет только одна цель — сразиться с Баки и победить.",
    "thumbnail": "https://www.kinopoisk.ru/film/1125417/posters/",
    "publisher": "Netflix",
    "age_restriction": "EIGHTEEN_PLUS",
    "cast_members": [
      {
        "id": "1a66d90d-f79e-46cf-b5fd-f759abae26e0",
        "employee_full_name": "Джонни Деп",
        "role_name": "Актёр"
      }
    ]
}));

export const deleteFilm = jest.fn(()=>Promise.resolve());

export const createEpisode = jest.fn(()=>Promise.resolve({
    "id": "fa41d538-4587-4dd2-b006-b906d19c3db0",
    "title": "Возращение бога",
    "s3_bucket_name": "http://{address}:{port}/api/v1/bucket/films/file/{content-uuid}/{episode-uuid}/film.mp4",
    "episode_num": 1,
    "description": "В этом эпизоде бог вернулся",
    "season_num": 1,
    "content_id": "cfb4e3bc-6bb6-46d8-943e-b32c0056e37f",
    "status": "ERROR"
}));

export const getPersonalList = jest.fn(()=>Promise.resolve([
    {
        "id": "0247c06b-1dc0-409a-9622-31c55e79b358",
        "year": 2024,
        "title": "Планета обезьян: Новое царство",
        "quality": "P1080",
        "genre": "ACTION_FILM",
        "category": "MOVIE",
        "description": "После падения человеческой цивилизации обезьяны построили свое общество, но столкнулись с новыми угрозами, угрожающими их существованию. Лидер Цезарь борется за будущее своего народа.",
        "thumbnail": null,
        "publisher": "20th Century Studios",
        "age_restriction": "SIXTEEN_PLUS",
        "cast_members": [
            {
                "id": "6f29c24e-8b0b-4890-86e9-40682dcbd650",
                "employee_full_name": "Джуди Грир",
                "role_name": "Зира"
            },
            {
                "id": "506ac869-29c8-4aab-a9aa-8ce8a32ce9e2",
                "employee_full_name": "Фрея Аллан",
                "role_name": "Выжившая из человеческого поселения"
            },
            {
                "id": "44165bc6-efb0-4134-9ff2-57cc24011319",
                "employee_full_name": "Оуэн Тиг",
                "role_name": "Молодой лидер обезьян"
            },
            {
                "id": "8184422c-4cad-4d67-84d7-0d089cb366f7",
                "employee_full_name": "Энди Серкис",
                "role_name": "Цезарь (захват движения)"
            }
        ]
    }
]));

export const addToPersonalList = jest.fn(()=>Promise.resolve({
    "personal_list_id": "cfb4e3bc-6bb6-46d8-943e-b32c0056e37f",
    "user_id": "fa41d538-4587-4dd2-b006-b906d19c3db0",
    "content_id": "f884bb0c-225e-4a6d-af6e-f87ba6c3600c"
}));

export const removeFromPersonalList = jest.fn(()=>Promise.resolve());

export const changePassword = jest.fn(()=>Promise.resolve({
    "login": "Greed",
    "email": "greed2003@mail.ru",
    "user_id": "ee48c2c9-e39e-4014-ae1f-1ae45193d62a",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
}));

export const deleteUser = jest.fn(()=>Promise.resolve());

export const getEpisodes = jest.fn(()=>Promise.resolve([
    {
        "id": "94227d2c-27cf-4005-82a5-6504bf6e9051",
        "title": "Трейлер 1",
        "s3_bucket_name": null,
        "episode_num": 1,
        "description": "Короткий видеоролик, созданный для рекламы",
        "season_num": 1,
        "content_id": "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
        "status": "UPLOADED"
    },
    {
        "id": "013967f5-1b31-4e08-a245-473685dd21ba",
        "title": "Эпизод",
        "s3_bucket_name": null,
        "episode_num": 3,
        "description": "уафыафыва",
        "season_num": 1,
        "content_id": "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
        "status": "UPLOADED"
    },
]));

export const updateFilm = jest.fn(()=>Promise.resolve());

export const updateEpisode = jest.fn(()=>Promise.resolve());

export const uploadImage = jest.fn(()=>Promise.resolve());

export const uploadVideo = jest.fn(()=>Promise.resolve());

export const deleteEpisode = jest.fn(()=>Promise.resolve());

export const grantToAdmin = jest.fn(()=>Promise.resolve());
