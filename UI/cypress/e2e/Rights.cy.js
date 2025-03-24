import { TEST_USER_DATA } from "../fixtures/e2e_test_config";

describe("Granting admin rights", () => {

    const NONEXISTENT_USER_LOGIN = "test_login123"

    beforeEach(() => {
        cy.deleteTestUser();
        cy.logInTestAdmin();
        cy.visit("/");
        cy.get('a').contains(/Grant admin/i).click();
    })

    it("grants rights to existing user", () => {
        cy.createTestUser();
        cy.get('input[placeholder="Login"]').type(TEST_USER_DATA.login);
        cy.get('button').contains(/Submit/i).click();

        cy.contains(/Admin rights successfully granted/i).should('be.visible');
        
        cy.logInTestUser();
        cy.visit('/');
        cy.get('a').contains(TEST_USER_DATA.login).should('be.visible');
        cy.get('a').contains(/Grant admin/i).should('be.visible');
    })

    it("grants rights to existing user", () => {
        cy.createTestUser();
        cy.get('input[placeholder="Login"]').type(NONEXISTENT_USER_LOGIN);
        cy.get('button').contains(/Submit/i).click();

        cy.contains(/User not found/i).should('be.visible');
    })
})