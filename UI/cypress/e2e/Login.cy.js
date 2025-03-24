import { TEST_USER_DATA } from "../fixtures/e2e_test_config";

describe('Authentication', () => {
  beforeEach(() => {
    cy.createTestUser();
    cy.visit('/login')
  })

  it('logs in existing user', () => {
    cy.get('input[placeholder="Login or email"]').type(TEST_USER_DATA.email);
    cy.get('input[placeholder="Enter the password"]').type(TEST_USER_DATA.password);
    cy.get('button').contains('Sign in').click();

    cy.url().should('include', '/films');
  })

  it('does not log in nonexistent user', () => {
    cy.get('input[placeholder="Login or email"]').type("NonexistingUser");
    cy.get('input[placeholder="Enter the password"]').type("WrongPassword1!");
    cy.get('button').contains('Sign in').click();

    cy.contains('Incorrect login or password!').should("be.visible");
  })

  it('lets user in after two failed attempts', () => {
    cy.get('input[placeholder="Login or email"]').type(TEST_USER_DATA.login);
    cy.get('input[placeholder="Enter the password"]').type("WrongPassword1!");

    cy.get('button').contains('Sign in').click();
    cy.contains('Incorrect login or password!').should("be.visible");
    
    cy.get('button').contains('Sign in').click();
    cy.contains('Incorrect login or password!').should("be.visible");

    cy.get('input[placeholder="Enter the password"]').clear().type(TEST_USER_DATA.password);
    cy.get('button').contains('Sign in').click();

    cy.url().should('include', '/films');
  })

  it('times out user for too many log in attempts', () => {
    cy.get('input[placeholder="Login or email"]').type(TEST_USER_DATA.login);
    cy.get('input[placeholder="Enter the password"]').type("WrongPassword1!");

    cy.get('button').contains('Sign in').click();
    cy.get('button').contains('Sign in').click();
    cy.get('button').contains('Sign in').click();
    cy.get('button').contains('Sign in').click();

    cy.contains('Max amount of attempts reached!').should("be.visible");
  })
})