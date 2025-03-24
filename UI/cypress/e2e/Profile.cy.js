import { TEST_USER_DATA, TEST_USER_NEW_PASSWORD } from "../fixtures/e2e_test_config";

describe("View profile page", () => {
    beforeEach(() => {
        cy.deleteTestUser();
        cy.createTestUser();
        cy.logInTestUser();
        cy.visit('/');
        cy.get('a').contains(TEST_USER_DATA.login).click();
    })

    it("shows the user profile", () => {
        cy.get('h1').contains(TEST_USER_DATA.login).should("be.visible");
        cy.get('h3').contains(TEST_USER_DATA.email).should("be.visible");
        cy.get('h3').contains(TEST_USER_DATA.first_name).should("be.visible");
        cy.get('h3').contains(TEST_USER_DATA.last_name).should("be.visible");
    })
})

describe("Changing password on profile page", () => {

    const WRONG_NEW_PASSWORD = "aaaaaaaaaaaaaaaa"

    beforeEach(() => {
        cy.deleteTestUser();
        cy.createTestUser();
        cy.logInTestUser();
        cy.visit('/');
        cy.get('a').contains(TEST_USER_DATA.login).click();
    })

    it("changes the password if correct", () => {
        cy.get('input[placeholder="Enter the password"]').type(TEST_USER_NEW_PASSWORD);
        cy.get('input[placeholder="Repeat the password"]').type(TEST_USER_NEW_PASSWORD);
        cy.get('button').contains(/Submit/i).click();
        
        cy.get('button').contains(/Log out/i).click();
        cy.visit("/login")

        cy.get('input[placeholder="Login or email"]').type(TEST_USER_DATA.email);
        cy.get('input[placeholder="Enter the password"]').type(TEST_USER_DATA.password);
        cy.get('button').contains('Sign in').click();

        cy.contains(/Incorrect login or password!|Max amount of attempts reached!/g).should("be.visible");
    })

    it("does not changes the password if incorrect", () => {
        cy.get('input[placeholder="Enter the password"]').type(WRONG_NEW_PASSWORD);
        cy.get('input[placeholder="Repeat the password"]').type(WRONG_NEW_PASSWORD);
        cy.get('button').contains(/Submit/i).click();
        
        cy.contains(/Password should include at least one number/i).should("be.visible");
    })
})

describe("Deleting account on profile page", () => {

    beforeEach(() => {
        cy.deleteTestUser();
        cy.createTestUser();
        cy.logInTestUser();
        cy.visit('/');
        cy.get('a').contains(TEST_USER_DATA.login).click();
    })

    it("deletes account successfully", () => {
        cy.get('button').contains(/Delete account/i).click();
        cy.get('button').contains(/Yes/i).click();

        cy.visit("/login")

        cy.get('input[placeholder="Login or email"]').type(TEST_USER_DATA.email);
        cy.get('input[placeholder="Enter the password"]').type(TEST_USER_DATA.password);
        cy.get('button').contains('Sign in').click();

        cy.contains(/Incorrect login or password!/i).should("be.visible");
    })
})