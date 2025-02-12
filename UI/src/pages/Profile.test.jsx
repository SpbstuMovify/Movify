import React from "react";
import { screen, fireEvent, waitFor, act } from "@testing-library/react";
import { renderWithRouter } from "../configs/testConfig";
import Profile from "./Profile";
import '@testing-library/jest-dom';
import { useAuth } from "@contexts/AuthContext";
import * as AuthContext from '@contexts/AuthContext';
import * as api from '@services/api';

jest.mock("@services/api", () => require("../__mocks__/services/api"));
jest.mock("@contexts/AuthContext", () => require("../__mocks__/contexts/AuthContext"));
const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

describe("Profile Component", () => {
    const mockSetUserData = jest.fn();

    beforeEach(() => {
        jest.resetModules();
        jest.doMock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));
        const AuthContextReset = require('@contexts/AuthContext');
        AuthContext.useAuth = AuthContextReset.useAuth;
        jest.doMock('@services/api', () => require('../__mocks__/services/api'));
        const apiReset = require('@services/api');
        api.getUserById = apiReset.getUserById;
    });

    test("renders profile page with user info", async () => {
        renderWithRouter(<Profile />);
        await waitFor(() => expect(screen.getByText("last-name")).toBeInTheDocument());
        expect(screen.getByRole("profile-body")).toBeInTheDocument();
    });


    test("handles password change successfully", async () => {
        renderWithRouter(<Profile />);
    
        await waitFor(() => {expect(screen.getByText(/last-name/)).toBeInTheDocument();});
        const passwordField = await screen.findByPlaceholderText("Enter the password");
        const repeatField = await screen.findByPlaceholderText("Repeat the password");
        const submitButton = await screen.findByRole("button", { name: "Submit" });
        await act(async () => {
            fireEvent.change(passwordField, { target: { value: "NewPassword123!" } });
            fireEvent.change(repeatField, { target: { value: "NewPassword123!" } });
            fireEvent.click(submitButton);
        });
        expect(api.changePassword).toHaveBeenCalled();
        await waitFor(() => {
            expect(api.changePassword).toHaveBeenCalledWith(
                "NewPassword123!",
                "login",
                "email@email.em",
                "USER",
                "dummy-token"
            );
            expect(AuthContext.useAuth().setUserData).toHaveBeenCalled();
        });
    });

    test("displays error on password change failure with code 500", async () => {
        api.changePassword.mockRejectedValue({ response: { status: 500 } });

        renderWithRouter(<Profile />);

        await waitFor(() => {
            expect(screen.getByPlaceholderText("Enter the password")).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.change(screen.getByPlaceholderText("Enter the password"), {
                target: { value: "Pass12!" },
            });
            fireEvent.change(screen.getByPlaceholderText("Repeat the password"), {
                target: { value: "Pass12!" },
            });
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Server error occurred!")).toBeInTheDocument();
        });
    });

    test("displays error on password change failure with code other than 500", async () => {
        api.changePassword.mockRejectedValue({ response: { status: 501 } });

        renderWithRouter(<Profile />);

        await waitFor(() => {
            expect(screen.getByPlaceholderText("Enter the password")).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.change(screen.getByPlaceholderText("Enter the password"), {
                target: { value: "Pass12!" },
            });
            fireEvent.change(screen.getByPlaceholderText("Repeat the password"), {
                target: { value: "Pass12!" },
            });
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        await waitFor(() => {
            expect(screen.getByText(/There was an error changing the password\.\.\./)).toBeInTheDocument();
        });
    });

    test("opens confirmation modal for account deletion", async () => {
        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Delete account" }));
        });

        expect(await screen.findByText(/Are you sure you want to delete your account\?/)).toBeInTheDocument();
    });

    test("closes confirmation modal for account deletion", async () => {
        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Delete account" }));
        });

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "No" })).toBeInTheDocument()
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "No" }));
        });

        await waitFor(async () => {
            expect(screen.queryByRole("button", { name: "No" })).not.toBeInTheDocument()
        });
    });

    test("handles account deletion successfully", async () => {
        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Delete account" }));
        });

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Yes" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Yes" })); 
        });

        await waitFor(() => {
            expect(api.deleteUser).toHaveBeenCalledWith('68b1ba18-b97c-4481-97d5-debaf9616182', "dummy-token");
            expect(useAuth().clearUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/");
        });
    });

    test("displays error if account deletion fails", async () => {
        api.deleteUser.mockRejectedValue({ response: { status: 500 } });

        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Delete account" }));
        });

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Yes" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Yes" })); 
        });

        await waitFor(() => {
            expect(screen.getByText(/There was an error deleting the account!/)).toBeInTheDocument();
        });
    });

    test("displays error and clears userData if account deletion fails with 401", async () => {
        api.deleteUser.mockRejectedValue({ response: { status: 401 } });

        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Delete account" }));
        });

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Yes" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Yes" })); 
        });

        await waitFor(() => {
            expect(AuthContext.useAuth().clearUserData).toHaveBeenCalled();
            expect(screen.getByText(/There was an error deleting the account!/)).toBeInTheDocument();
        });
    });

    test("logs out user when clicking 'Log out'", async () => {
        renderWithRouter(<Profile />);

        await waitFor(async () => {
            expect(screen.getByRole("button", { name: "Log out" })).toBeInTheDocument();
        });

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Log out" }));
        });

        await waitFor(() => {
            expect(useAuth().clearUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/");
        });
    });
});
