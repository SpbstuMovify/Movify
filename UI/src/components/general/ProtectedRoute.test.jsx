import { render, screen } from "@testing-library/react"
import { RouterProvider, createMemoryRouter } from "react-router-dom";
import '@testing-library/jest-dom'
import routesConfig from "../../configs/routesConfig";
import * as AuthContext from '@contexts/AuthContext';

jest.clearAllMocks();
jest.mock('@contexts/AuthContext', () => require('../../__mocks__/contexts/AuthContext'));
jest.mock('@services/api', () => require('../../__mocks__/services/api'));

describe('App routing of protected components', () => {
    afterEach(() => {
        jest.resetModules();
        jest.doMock('@contexts/AuthContext', () => require('../../__mocks__/contexts/AuthContext'));
        const AuthContextReset = require('@contexts/AuthContext');
        AuthContext.useAuth = AuthContextReset.useAuth;
    });
    

    test('redirects to /login from /profile when not authenticated', async () => {
        AuthContext.useAuth().userData = undefined;
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/profile"]
        })
        render(<RouterProvider future={{
                v7_relativeSplatPath: true,
                v7_startTransition: true
            }}
            router={router} />);
        expect(await screen.findByText(/New to Movify\?/i)).toBeInTheDocument();
    });

    test('renders Profile on /profile when authenticated', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/profile"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        expect(await screen.findByRole("profile-body", {})).toBeInTheDocument();
    });

    test('redirects to /login from /favorites when not authenticated', async () => {
        AuthContext.useAuth().userData = undefined;
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/favorites"]
        })
        render(<RouterProvider future={{
                v7_relativeSplatPath: true,
                v7_startTransition: true
            }}
            router={router} />);
        expect(await screen.findByText(/New to Movify\?/i)).toBeInTheDocument();
    });

    test('renders Favorites on /favorites when authenticated', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/favorites"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        expect(await screen.findByText(/Your favorite films and series/i)).toBeInTheDocument();
    });

    test('redirects to /login from /rights when authenticated as user', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/rights"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        expect(await screen.findByText(/New to Movify\?/i)).toBeInTheDocument();
    });

    test('redirects to /login from /rights when not authenticated', async () => {
        AuthContext.useAuth().userData = undefined;
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/rights"]
        })
        render(<RouterProvider future={{
                v7_relativeSplatPath: true,
                v7_startTransition: true
            }}
            router={router} />);
        expect(await screen.findByText(/New to Movify\?/i)).toBeInTheDocument();
    });

    test('renders Rights when authenticated as admin', async () => {
        AuthContext.useAuth().userData = {
            "login": "login",
            "email": "email@email.em",
            "user_id": "68b1ba18-b97c-4481-97d5-debaf9616182",
            "role": "ADMIN",
            "token": "dummy-token"
        };
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/rights"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        expect(await screen.findByText(/Grant Administrator Rights/i)).toBeInTheDocument();
    });
  
});