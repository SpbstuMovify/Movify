// ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'
import { TEST_ADMIN_DATA } from "../fixtures/e2e_test_config"
import axios from 'axios';

export const apiBaseURL = 'http://localhost:8090';

const api = axios.create({
  baseURL: `${apiBaseURL}/v1`,
});

const createAdminUser = async () => {
    try{
        await api.post('/users/register', TEST_ADMIN_DATA);
        await setTimeout(10000);
    }
    catch(err) {
        console.error(err);
    }
}

createAdminUser();