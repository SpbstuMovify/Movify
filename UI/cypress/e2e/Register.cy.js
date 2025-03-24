import { TEST_USER_DATA } from "../fixtures/e2e_test_config";

describe('Registration', () => {
  beforeEach(() => {
    cy.deleteTestUser();
    cy.visit('/register')
  })

  it('successfully registers a new user', () => {
    cy.get('input[placeholder="Login"]').type(TEST_USER_DATA.login);
    cy.get('input[placeholder="Enter the password"]').type(TEST_USER_DATA.password);
    cy.get('input[placeholder="Repeat the password"]').type(TEST_USER_DATA.password);
    cy.get('input[placeholder="Email"]').type(TEST_USER_DATA.email);
    cy.get('#terms').check();
    cy.get('button').contains('Create').click();
    
    cy.url().should('include', '/films');
  })

  it('doesn\'t allow registering an existing user', () => {
    cy.createTestUser();
    
    cy.get('input[placeholder="Login"]').type(TEST_USER_DATA.login);
    cy.get('input[placeholder="Enter the password"]').type(TEST_USER_DATA.password);
    cy.get('input[placeholder="Repeat the password"]').type(TEST_USER_DATA.password);
    cy.get('input[placeholder="Email"]').type(TEST_USER_DATA.email);
    cy.get('#terms').check();
    cy.get('button').contains('Create').click();

    cy.contains(/User with this login and password already exists!/i).should('be.visible');
  })

})