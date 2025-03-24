import { TEST_FILM_DATA } from "../fixtures/e2e_test_config";


describe("Search films", () => {
    beforeEach(() => {
        cy.clearTestFilms();
        cy.createTestFilm();
        cy.visit("/films");
    })

    it("shows the required film", () => {
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('#age-restriction').select(TEST_FILM_DATA.age_restriction);
        cy.get('#genre').select(TEST_FILM_DATA.genre);
        cy.get('#year').type(TEST_FILM_DATA.year);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('h2[data-test-cy="film-title"]').contains(TEST_FILM_DATA.title).should("be.visible");
    })
})

describe("Creating films", () => {
    beforeEach(() => {
        cy.clearTemplateFilms();
        cy.logInTestAdmin();
        cy.visit("/films");
    })

    it("creates a template film", () => {
        cy.get('button').contains(/Add new film/i).click();
        cy.get('input[placeholder="Search..."]').type("New film");
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('h2[data-test-cy="film-title"]').contains(/New film/i).should("be.visible");
    })
})
