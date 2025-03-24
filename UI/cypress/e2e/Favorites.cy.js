import { TEST_FILM_DATA } from "../fixtures/e2e_test_config";

describe("Viewing favorites", () => {
    beforeEach(() => {
        cy.wait(20000);
        cy.deleteTestUser();
        cy.clearTestFilms();

        cy.createTestFilm();

        cy.createTestUser();
        cy.logInTestUser();

        cy.addTestFilmToFavorites();
        
        cy.visit("/")
        cy.get("a").contains("Favorites").click();
    })

    it("shows the added film on favorites page", () => {
        cy.get('h2[data-test-cy="film-title"]').contains(TEST_FILM_DATA.title).should("be.visible");
    })
})

describe("Removing from favorites", () => {
    beforeEach(() => {
        cy.deleteTestUser();
        cy.clearTestFilms();

        cy.createTestFilm();

        cy.createTestUser();
        cy.logInTestUser();

        cy.addTestFilmToFavorites();
        
        cy.visit("/")
        cy.get("a").contains("Favorites").click();
    })
    
    it("removes the added film on favorites page", () => {
        cy.get('button[data-test-cy="remove-from-favorites"]').click();
        cy.contains("No films found...").should("be.visible");
    })
})