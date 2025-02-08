import React from "react";
import { screen, fireEvent, waitFor, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Login from "./SignInScreen";
import { login as mockLoginUser } from "../services/api";
import { useAuth as mockUseAuth } from "../contexts/AuthContext";
import { renderWithRouter } from "../configs/testConfig";

jest.mock('@services/api', () => require('../__mocks__/services/api'));
jest.mock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
    ...jest.requireActual("react-router-dom"),
    useNavigate: () => mockNavigate,
}));

describe("Login Component", () => {
    const originalError = console.error;

    beforeAll(() => {
        jest.spyOn(console, "error").mockImplementation((msg, ...args) => {
            if (typeof msg === "string" && msg.includes("act(...)")) {
                return; // Suppress act() warnings
            }
            originalError(msg, ...args); // Keep other errors
        });
    });

    afterAll(() => {
        console.error.mockRestore();
    });

    test("renders login form elements", () => {
        renderWithRouter(<Login />);

        expect(screen.getByPlaceholderText("Login or email")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Enter the password")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: "Sign in" })).toBeInTheDocument();
        expect(screen.getByRole("button", { name: "Sign Up" })).toBeInTheDocument();
    });

    test("allows user to type in login and password fields", async () => {
        renderWithRouter(<Login />);

        const loginInput = screen.getByPlaceholderText("Login or email");
        const passwordInput = screen.getByPlaceholderText("Enter the password");

        await act(async () => {
            await userEvent.type(loginInput, "testuser");
            await userEvent.type(passwordInput, "password123");
        });

        expect(loginInput).toHaveValue("testuser");
        expect(passwordInput).toHaveValue("password123");
    });

    test("shows error when login is missing", async () => {
        renderWithRouter(<Login />);

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        expect(await screen.findByText("Please enter the login or email!")).toBeInTheDocument();
    });

    test("shows error when password is missing", async () => {
        renderWithRouter(<Login />);


        const loginInput = screen.getByPlaceholderText("Login or email");

        await act(async () => {
            await userEvent.type(loginInput, "testuser");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        expect(await screen.findByText("Please enter the password!")).toBeInTheDocument();
    });

    test("submits login form and navigates to /films on success", async () => {
        const mockSetUserData = jest.fn();
        mockUseAuth.mockReturnValue({ setUserData: mockSetUserData });
        mockLoginUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });

        renderWithRouter(<Login />);


        const loginInput = screen.getByPlaceholderText("Login or email");
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const signInButton = screen.getByRole("button", { name: "Sign in" });

        await act(async () => {
            await userEvent.type(loginInput, "testuser");
            await userEvent.type(passwordInput, "password123");
            await fireEvent.click(signInButton);
        });

        await waitFor(() => {
            expect(mockLoginUser).toHaveBeenCalledWith("testuser", "password123", expect.any(String));
            expect(mockSetUserData).toHaveBeenCalled();
        });

        expect(screen.getByText("Login successful!")).toBeInTheDocument();
    });

    test("displays error message for incorrect login (401)", async () => {
        mockLoginUser.mockRejectedValue({ response: { status: 401, data: { body: { detail: "Please try again" } } } });

        renderWithRouter(<Login />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "wronguser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "wrongpassword");
            await fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Incorrect login or password!")).toBeInTheDocument();
        });
    });

    test("displays error message when max attempts are reached (401)", async () => {
        mockLoginUser.mockRejectedValue({ response: { status: 401, data: { body: { detail: "Max amount of attempts reached" } } } });

        renderWithRouter(<Login />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "wronguser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "wrongpassword");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Max amount of attempts reached!")).toBeInTheDocument();
        });
    });

    test("displays error message for non-existent user (404)", async () => {
        mockLoginUser.mockRejectedValue({ response: { status: 404 } });

        renderWithRouter(<Login />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "nonexistentuser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "wrongpassword");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Incorrect login or password!")).toBeInTheDocument();
        });
    });

    test("displays error message for server error (500)", async () => {
        mockLoginUser.mockRejectedValue({ response: { status: 500 } });

        renderWithRouter(<Login />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "servererroruser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "password");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Server error occurred!")).toBeInTheDocument();
        });
    });

    test("displays generic error message for unknown error", async () => {
        mockLoginUser.mockRejectedValue({ response: { status: 999 } }); // Unknown status
    
        renderWithRouter(<Login />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "password123");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });
    
        await waitFor(() => {
            expect(screen.getByText("There was an error logging in!")).toBeInTheDocument();
        });
    });

    test("displays a loading indicator while logging in", async () => {
        mockLoginUser.mockImplementation(() => new Promise((resolve) => setTimeout(() => resolve({ token: "fakeToken" }), 1000)));

        renderWithRouter(<Login />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login or email"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "password123");
            fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
        });

        expect(screen.getByRole("img", { name: "" })).toBeInTheDocument();
    });

    test("navigates to register page when 'Sign Up' is clicked", async () => {
        renderWithRouter(<Login />);

        const signUpSpan = screen.getByText("Sign Up now.");
        
        await act(async () => {
            fireEvent.click(signUpSpan);
        });

        expect(mockNavigate).toHaveBeenCalledWith("/register");
    });

    test("navigates to register page when 'Sign Up' is clicked", async () => {
        renderWithRouter(<Login />);
    
        const signUpButton = screen.getByRole("button", { name: "Sign Up" });
        
        await act(async () => { 
            fireEvent.click(signUpButton);
        });
    
        expect(mockNavigate).toHaveBeenCalledWith("/register");
      });

});
