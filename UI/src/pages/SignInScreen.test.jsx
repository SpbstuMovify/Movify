import React from "react";
import { screen, fireEvent, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Login from "./SignInScreen";
import { login as mockLoginUser } from "../services/api";
import { useAuth as mockUseAuth } from "../contexts/AuthContext";
import { renderWithRouter } from "../configs/testConfig";
import '@testing-library/jest-dom';

jest.mock('@services/api', () => require('../__mocks__/services/api'));
jest.mock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));

const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

describe("Login Component", () => {
  beforeEach(() => {
    jest.clearAllMocks();
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
    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");

    await userEvent.type(loginInput, "testuser");
    await userEvent.type(passwordInput, "password123");

    expect(loginInput).toHaveValue("testuser");
    expect(passwordInput).toHaveValue("password123");
  });

  test("shows error when login is missing", async () => {
    renderWithRouter(<Login />);
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
    expect(await screen.findByText("Please enter the login or email!")).toBeInTheDocument();
  });

  test("shows error when password is missing", async () => {
    renderWithRouter(<Login />);
    const loginInput = await screen.findByPlaceholderText("Login or email");
    await userEvent.type(loginInput, "testuser");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));
    expect(await screen.findByText("Please enter the password!")).toBeInTheDocument();
  });

  test("submits login form and navigates to /films on success", async () => {
    const mockSetUserData = jest.fn();
    mockUseAuth().setUserData = mockSetUserData;
    mockLoginUser.mockResolvedValue({ token: "fakeToken", login: "testuser" });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");

    await userEvent.type(loginInput, "testuser");
    await userEvent.type(passwordInput, "password123");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    await waitFor(() => {
      expect(mockLoginUser).toHaveBeenCalledWith("testuser", "password123", expect.any(String));
      expect(mockSetUserData).toHaveBeenCalled();
      expect(screen.getByText("Login successful!")).toBeInTheDocument();
    });
  });

  test("displays error message for incorrect login (401)", async () => {
    mockLoginUser.mockRejectedValue({
      response: { status: 401, data: { body: { detail: "Please try again" } } },
    });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "wronguser");
    await userEvent.type(passwordInput, "wrongpassword");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("Incorrect login or password!")).toBeInTheDocument();
  });

  test("displays error message when max attempts are reached (401)", async () => {
    mockLoginUser.mockRejectedValue({
      response: { status: 401, data: { body: { detail: "Max amount of attempts reached" } } },
    });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "wronguser");
    await userEvent.type(passwordInput, "wrongpassword");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("Max amount of attempts reached!")).toBeInTheDocument();
  });

  test("displays error message for non-existent user (404)", async () => {
    mockLoginUser.mockRejectedValue({ response: { status: 404 } });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "nonexistentuser");
    await userEvent.type(passwordInput, "wrongpassword");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("Incorrect login or password!")).toBeInTheDocument();
  });

  test("displays error message for server error (500)", async () => {
    mockLoginUser.mockRejectedValue({ response: { status: 500 } });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "servererroruser");
    await userEvent.type(passwordInput, "password");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("Server error occurred!")).toBeInTheDocument();
  });

  test("displays generic error message for unknown error", async () => {
    mockLoginUser.mockRejectedValue({ response: { status: 999 } });

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "testuser");
    await userEvent.type(passwordInput, "password123");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByText("There was an error logging in!")).toBeInTheDocument();
  });

  test("displays a loading indicator while logging in", async () => {
    mockLoginUser.mockImplementation(
      () => new Promise((resolve) => setTimeout(() => resolve({ token: "fakeToken" }), 1000))
    );

    renderWithRouter(<Login />);

    const loginInput = await screen.findByPlaceholderText("Login or email");
    const passwordInput = await screen.findByPlaceholderText("Enter the password");
    await userEvent.type(loginInput, "testuser");
    await userEvent.type(passwordInput, "password123");
    fireEvent.click(screen.getByRole("button", { name: "Sign in" }));

    expect(await screen.findByRole("img", { name: "" })).toBeInTheDocument();
  });

  test("navigates to register page when 'Sign Up now.' is clicked", async () => {
    renderWithRouter(<Login />);
    const signUpSpan = await screen.findByText("Sign Up now.");
    fireEvent.click(signUpSpan);
    expect(mockNavigate).toHaveBeenCalledWith("/register");
  });

  test("navigates to register page when 'Sign Up' is clicked", async () => {
    renderWithRouter(<Login />);
    const signUpButton = await screen.findByRole("button", { name: "Sign Up" });
    fireEvent.click(signUpButton);
    expect(mockNavigate).toHaveBeenCalledWith("/register");
  });
});
