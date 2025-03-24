// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })

import { uploadVideo } from "../../src/services/api";
import { TEST_USER_DATA, TEST_ADMIN_DATA, TEST_FILM_DATA, TEST_USER_NEW_PASSWORD } from "../fixtures/e2e_test_config";

Cypress.Commands.add('createTestUser', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/register`,
        body: TEST_USER_DATA,
        failOnStatusCode: false
    });
})

const logInUser = (login, password) => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: login,
            password: password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((response) => {
        window.localStorage.setItem('userData', JSON.stringify(response.body))
    });
}

Cypress.Commands.add('logInTestUser', () => {
    logInUser(TEST_USER_DATA.login, TEST_USER_DATA.password)
})

Cypress.Commands.add('logInTestAdmin', () => {
    logInUser(TEST_ADMIN_DATA.login, TEST_ADMIN_DATA.password)
})

const deleteUser = (id, token) =>{
    cy.request({
        method: 'DELETE',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/${id}`,
        headers: {
            Authorization: `${token}`,
        },
        failOnStatusCode: false
    });
}

Cypress.Commands.add('deleteTestUser', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_USER_DATA.login,
            password: TEST_USER_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        },
        failOnStatusCode: false
    }).then((response) => {
        if (response.status === 401)
        {
            cy.request({
                method: 'POST',
                url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
                body: {
                    login: TEST_USER_DATA.login,
                    password: TEST_USER_NEW_PASSWORD,
                    search_type: "LOGIN"
                },
                headers: {
                    ip: "1.1.1.1"
                },
                failOnStatusCode: false
            }).then((response) => {
                if (response.status !== 404)
                {
                    deleteUser(response.body.user_id, response.body.token)
                }
            });
        }
        if (response.status !== 404)
        {
            deleteUser(response.body.user_id, response.body.token)
        }
    });
})


Cypress.Commands.add('deleteAuthorizedUser', () => {
    cy.window().then(async (win) => {
        const retrievedUserData = win.localStorage.getItem('userData');
        console.log(retrievedUserData);
        if (retrievedUserData) {
            const userData = JSON.parse(retrievedUserData);
            deleteUser(userData.id, userData.token)
        }
    })
})

Cypress.Commands.add('createTestFilm', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_ADMIN_DATA.login,
            password: TEST_ADMIN_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((response) => {
        cy.request({
            method: 'POST',
            url: `${Cypress.env('apiBaseUrl')}/v1/contents`,
            body: TEST_FILM_DATA,
            headers: {
                Authorization: response.body.token
            }
        })
    });
})

Cypress.Commands.add('clearTestFilms', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_ADMIN_DATA.login,
            password: TEST_ADMIN_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((adminUserDataResponse) => {
        cy.request({
            method: 'GET',
            url: `${Cypress.env('apiBaseUrl')}/v1/contents?page_size=200&page_number=0`,
        }).then((filmsResponse)=>{
            filmsResponse.body.filter(film => film.title === 'TEST_FILM' || film.title === "TEST_TITLE").forEach((film) => {
                cy.request({
                    method: 'DELETE',
                    url: `${Cypress.env('apiBaseUrl')}/v1/contents/${film.id}`,
                    headers: {
                        Authorization: adminUserDataResponse.body.token
                    }
                });
            });
        })
    });
})

Cypress.Commands.add('addTestFilmToFavorites', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_USER_DATA.login,
            password: TEST_USER_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((userDataResponse) => {
        cy.request({
            method: 'GET',
            url: `${Cypress.env('apiBaseUrl')}/v1/contents?page_size=200&page_number=0`,
        }).then((filmsResponse)=>{
            const testFilm = filmsResponse.body.filter(film => film.title === 'TEST_FILM')[0]
            cy.request({
                method: 'POST',
                url: `${Cypress.env('apiBaseUrl')}/v1/users/personal-list`,
                body: {
                    "content_id": testFilm.id,
                    "user_id": userDataResponse.body.user_id
                },
                headers: {
                    Authorization: `${userDataResponse.body.token}`,
                },
            })
        })
    });
})

Cypress.Commands.add('createTestFilmEpisode', (seasonNum, episodeNum) => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_ADMIN_DATA.login,
            password: TEST_ADMIN_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((adminUserDataResponse) => {
        cy.request({
            method: 'GET',
            url: `${Cypress.env('apiBaseUrl')}/v1/contents?page_size=200&page_number=0`,
        }).then((filmsResponse)=>{
            const testFilm = filmsResponse.body.filter(film => film.title === 'TEST_FILM')[0]
            cy.request({
                method: 'POST',
                url: `${Cypress.env('apiBaseUrl')}/v1/episodes`,
                body: {
                    "episode_num": episodeNum,
                    "season_num": seasonNum,
                    "title": "New Episode",
                    "description": "",
                    "content_id": testFilm.id
                },
                headers: {
                    Authorization: `${adminUserDataResponse.body.token}`,
                },
                failOnStatusCode: false
            })
        })
    });
})

Cypress.Commands.add('clearTemplateFilms', () => {
    cy.request({
        method: 'POST',
        url: `${Cypress.env('apiBaseUrl')}/v1/users/login`,
        body: {
            login: TEST_ADMIN_DATA.login,
            password: TEST_ADMIN_DATA.password,
            search_type: "LOGIN"
        },
        headers: {
            ip: "1.1.1.1"
        }
    }).then((adminUserDataResponse) => {
        cy.request({
            method: 'GET',
            url: `${Cypress.env('apiBaseUrl')}/v1/contents?page_size=200&page_number=0`,
        }).then((filmsResponse)=>{
            filmsResponse.body.filter(film => film.title === 'New film').forEach((film) => {
                cy.request({
                    method: 'DELETE',
                    url: `${Cypress.env('apiBaseUrl')}/v1/contents/${film.id}`,
                    headers: {
                        Authorization: adminUserDataResponse.body.token
                    }
                });
            });
        })
    });
})