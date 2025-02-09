import { screen } from "@testing-library/react"
import '@testing-library/jest-dom'
import * as AuthContext from '@contexts/AuthContext';
import Navigation from "./Navigation";
import { renderWithRouter } from '../../configs/testConfig';

jest.clearAllMocks();
jest.mock('@contexts/AuthContext', () => require('../../__mocks__/contexts/AuthContext'));

describe('Navigation element', () => {
    afterEach(() => {
        jest.resetModules();
        jest.doMock('@contexts/AuthContext', () => require('../../__mocks__/contexts/AuthContext'));
        const AuthContextReset = require('@contexts/AuthContext');
        AuthContext.useAuth = AuthContextReset.useAuth;
    });
    
    test('renders logo, Films and SignIn link for unathenticated user', async () => {
        AuthContext.useAuth.mockReturnValue({
            userData: undefined,
            checkUserData: jest.fn(),
            clearUserData: jest.fn(),
        });
        renderWithRouter(<Navigation />);
        expect(screen.getByRole("link", { name: /films/i })).toBeInTheDocument();
        expect(screen.getByRole("link", { name: /sign in/i })).toBeInTheDocument();
        expect(screen.getByTestId("nav-logo")).toBeInTheDocument();
    });

    test('does not render incorrect links for unathenticated user', async () => {
        AuthContext.useAuth.mockReturnValue({
            userData: undefined,
            checkUserData: jest.fn(),
            clearUserData: jest.fn(),
        });
        renderWithRouter(<Navigation />);

        expect(screen.queryByRole("link", { name: /favorites/i })).not.toBeInTheDocument();
        expect(screen.queryByRole("link", { name: /grant admin/i })).not.toBeInTheDocument();
        expect(screen.queryByTestId('profile-link')).not.toBeInTheDocument();
    });

    test('renders Favorites and Profile links for authenticated user', async () => {
        renderWithRouter(<Navigation />);

        expect(screen.queryByRole("link", { name: /favorites/i })).toBeInTheDocument();
        expect(screen.queryByRole('link', { name: /login/i })).toBeInTheDocument();
    });

    test('does not render Grant admin link for common authenticated user', async () => {
        renderWithRouter(<Navigation />);

        expect(screen.queryByRole("link", { name: /grant admin/i })).not.toBeInTheDocument();
    });

    test('renders Grant Admin link for authenticated ADMIN user', async () => {
        AuthContext.useAuth.mockReturnValue({
            userData: {
                login: "login",
                email: "email@email.em",
                user_id: "user-id",
                role: "ADMIN",
                token: "dummy-token"
            },
            checkUserData: jest.fn(),
            clearUserData: jest.fn(),
        });
        renderWithRouter(<Navigation />);

        expect(screen.queryByRole("link", { name: /favorites/i })).toBeInTheDocument();
        expect(screen.queryByRole("link", { name: /grant admin/i })).toBeInTheDocument();
        expect(screen.queryByRole('link', { name: /login/i })).toBeInTheDocument();
    });
})