import React from "react";
import { screen, fireEvent, waitFor, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Register from "./Register";
import { register as mockRegisterUser } from "../services/api";
import { useAuth as mockUseAuth } from "../contexts/AuthContext";
import { renderWithRouter } from "../configs/testConfig";

jest.clearAllMocks();
jest.mock('@services/api', () => require('../__mocks__/services/api'));
jest.mock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
    ...jest.requireActual("react-router-dom"),
    useNavigate: () => mockNavigate,
}));

describe("Register Component", () => {
    const mockSetUserData = jest.fn();
    const originalError = console.error;

    afterEach(() => {
        console.error.mockRestore();
    });

    beforeEach(() => {
        jest.spyOn(console, "error").mockImplementation((msg, ...args) => {
            if (typeof msg === "string" && msg.includes("act(...)")) {
                return; // Suppress act() warnings
            }
            originalError(msg, ...args); // Keep other errors
        });
        mockUseAuth().setUserData = mockSetUserData;
    });

    test("renders form elements", () => {
        renderWithRouter(<Register />);

        expect(screen.getByPlaceholderText("Login")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Email")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Enter the password")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Repeat the password")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: "Create" })).toBeInTheDocument();
    });

    test("allows user to type input fields", async () => {
        renderWithRouter(<Register />);

        const loginInput = screen.getByPlaceholderText("Login");
        const emailInput = screen.getByPlaceholderText("Email");
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const repeatPasswordInput = screen.getByPlaceholderText("Repeat the password");

        await act(async () => {
            await userEvent.type(loginInput, "testuser");
            await userEvent.type(emailInput, "testuser@example.com");
            await userEvent.type(passwordInput, "Password1!");
            await userEvent.type(repeatPasswordInput, "Password1!");
        });

        expect(loginInput).toHaveValue("testuser");
        expect(emailInput).toHaveValue("testuser@example.com");
        expect(passwordInput).toHaveValue("Password1!");
        expect(repeatPasswordInput).toHaveValue("Password1!");
    });

    test("shows error when login is missing", async () => {
        renderWithRouter(<Register />);

        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });

        expect(await screen.findByText("Please enter the login!")).toBeInTheDocument();
    });

    test("shows error for invalid email format", async () => {
        renderWithRouter(<Register />);

        const emailInput = screen.getByPlaceholderText("Email");

        await act(async () => {
            await userEvent.type(emailInput, "invalid-email");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });

        expect(await screen.findByText("Incorrect email! Example: email@email.com")).toBeInTheDocument();
    });

    test("shows error if passwords do not match", async () => {
        renderWithRouter(<Register />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Password1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Mismatch1!");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });

        expect(await screen.findByText("Passwords do not match")).toBeInTheDocument();
    });

    test("shows error if password does not contain a number", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "Password!");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password should include at least one number")).toBeInTheDocument();
    });

    test("shows error if password does not contain a special symbol", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "Password1");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password should include at least one special symbol")).toBeInTheDocument();
    });

    test("shows error if password does not contain a lowercase letter", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "PASSWORD1!");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password should include at least one lowercase latin letter")).toBeInTheDocument();
    });

    test("shows error if password does not contain an uppercase letter", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "password1!");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password should include at least one uppercase latin letter")).toBeInTheDocument();
    });

    test("shows error if password is not long enough (5 symbols)", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "passw");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password must be from 6 to 32 symbols long")).toBeInTheDocument();
    });

    test("shows error if password is too long (33 symbols)", async () => {
        renderWithRouter(<Register />);
    
        const passwordInput = screen.getByPlaceholderText("Enter the password");
    
        await act(async () => {
            await userEvent.type(passwordInput, "W2!wwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("Password must be from 6 to 32 symbols long")).toBeInTheDocument();
    });

    test("submits form and navigates to /films on success when password is 6 symbols long", async () => {
        mockRegisterUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });
    
        renderWithRouter(<Register />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "test@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Pass1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Pass1!");
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            await fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        await waitFor(() => {
            expect(mockRegisterUser).toHaveBeenCalledWith(
                "test@example.com",
                "Password1!",
                "testuser",
                "", "" 
            );
            expect(mockSetUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/films");
        });
    
        expect(screen.getByText("Registration successful!")).toBeInTheDocument();
    });

    test("submits form and navigates to /films on success when password is 32 symbols long", async () => {
        mockRegisterUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });
    
        renderWithRouter(<Register />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "test@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Passssssssssssssssssssssssssss1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Pass1!");
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            await fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        await waitFor(() => {
            expect(mockRegisterUser).toHaveBeenCalledWith(
                "test@example.com",
                "Password1!",
                "testuser",
                "", "" 
            );
            expect(mockSetUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/films");
        });
    
        expect(screen.getByText("Registration successful!")).toBeInTheDocument();
    });

    test("submits form and navigates to /films on success (password is 19 symbols long)", async () => {
        mockRegisterUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });
    
        renderWithRouter(<Register />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "test@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Passsssssssssword1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Passsssssssssword1!");
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            await fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        await waitFor(() => {
            expect(mockRegisterUser).toHaveBeenCalledWith(
                "test@example.com",
                "Password1!",
                "testuser",
                "", "" 
            );
            expect(mockSetUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/films");
        });
    
        expect(screen.getByText("Registration successful!")).toBeInTheDocument();
    });

    test("submits form and navigates to /films on success (password is 16 symbols long)", async () => {
        mockRegisterUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });
    
        renderWithRouter(<Register />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "test@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Passssssssword1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Passssssssword1!");
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            await fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        await waitFor(() => {
            expect(mockRegisterUser).toHaveBeenCalledWith(
                "test@example.com",
                "Password1!",
                "testuser",
                "", "" 
            );
            expect(mockSetUserData).toHaveBeenCalled();
            expect(mockNavigate).toHaveBeenCalledWith("/films");
        });
    
        expect(screen.getByText("Registration successful!")).toBeInTheDocument();
    });

    test("shows error for already existing user (400)", async () => {
        mockRegisterUser.mockRejectedValue({ response: { status: 400 } });
    
        renderWithRouter(<Register />);
    
        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "existinguser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "existing@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Password1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Password1!");
            
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });
    
        expect(await screen.findByText("User with this login and password already exists!")).toBeInTheDocument();
    });
    

    test("shows server error (500)", async () => {
        mockRegisterUser.mockRejectedValue({ response: { status: 500 } });

        renderWithRouter(<Register />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "existinguser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "existing@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Password1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Password1!");
            
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Server error occurred!")).toBeInTheDocument();
        });
    });

    test("handles unknown error", async () => {
        mockRegisterUser.mockRejectedValue({ response: { status: 999 } });

        renderWithRouter(<Register />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "existinguser");
            await userEvent.type(screen.getByPlaceholderText("Email"), "existing@example.com");
            await userEvent.type(screen.getByPlaceholderText("Enter the password"), "Password1!");
            await userEvent.type(screen.getByPlaceholderText("Repeat the password"), "Password1!");
            
            const termsCheckbox = screen.getByRole("checkbox", { name: "I agree to" });
            await userEvent.click(termsCheckbox);
    
            fireEvent.click(screen.getByRole("button", { name: "Create" }));
        });

        expect(await screen.findByText("There was an error creating an account!")).toBeInTheDocument();
    });

    test("opens and closes terms of service modal", async () => {
        jest.spyOn(console, "error").mockImplementation(() => {});
        renderWithRouter(<Register />);
    
        const termsButton = screen.getByRole("button", { name: "Movify Terms of Service" });
        await act(async () => {
            await userEvent.click(termsButton);
        });
        
        expect(await screen.findByRole("heading", {name: "Movify Terms of Service"})).toBeInTheDocument();

        const closeButton = await screen.findByRole("button", { name: "X" });
    
        await act(async () => {
            await userEvent.click(closeButton);
        });

        expect(screen.queryByRole("heading", {name: "Movify Terms of Service"})).not.toBeInTheDocument();
    });

    test("shows error when terms file cannot be loaded", async () => {
        jest.spyOn(console, "error").mockImplementation(() => {});
        global.fetch = jest.fn(() =>
            Promise.resolve({ ok: false })
        );
    
        renderWithRouter(<Register />);
    
        const termsButton = screen.getByRole("button", { name: "Movify Terms of Service" });
    
        await act(async () => {
            userEvent.click(termsButton);
        });
    
        expect(await screen.findByText("Failed to load the file.")).toBeInTheDocument();
    });
    

    test("navigates to login page when 'Click here to sign in.' is clicked", async () => {
        renderWithRouter(<Register />);

        const signInLink = screen.getByText("Click here to sign in.");

        await act(async () => {
            fireEvent.click(signInLink);
        });

        expect(mockNavigate).toHaveBeenCalledWith("/login");
    });

    test("navigates to login page when 'Sign In' is clicked", async () => {
        renderWithRouter(<Register />);
    
        const signInButton = screen.getByRole("button", { name: "Sign In" });
    
        await act(async () => {
            userEvent.click(signInButton);
        });
    
        expect(mockNavigate).toHaveBeenCalledWith("/login");
    });
});
