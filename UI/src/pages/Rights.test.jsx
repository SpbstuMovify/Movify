import React from "react";
import { screen, fireEvent, waitFor, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Rights from "./Rights";
import { getUserByLogin as mockGetUserByLogin, grantToAdmin as mockGrantToAdmin } from "../services/api";
import { useAuth as mockUseAuth } from "../contexts/AuthContext";
import { renderWithRouter } from "../configs/testConfig";

jest.clearAllMocks();
jest.mock("@services/api", () => require("../__mocks__/services/api"));
jest.mock("@contexts/AuthContext", () => require("../__mocks__/contexts/AuthContext"));

describe("Rights Component", () => {
    const mockUserData = { token: "fakeToken" };

    beforeEach(() => {
        jest.clearAllMocks();
        mockUseAuth().userData = mockUserData;
    });

    test("renders form elements correctly", () => {
        renderWithRouter(<Rights />);

        expect(screen.getByPlaceholderText("Login")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: "Submit" })).toBeInTheDocument();
    });

    test("allows user to type a login", async () => {
        renderWithRouter(<Rights />);
        const loginInput = screen.getByPlaceholderText("Login");

        await act(async () => {
            await userEvent.type(loginInput, "testuser");
        });

        expect(loginInput).toHaveValue("testuser");
    });

    test("shows an error if login is missing", async () => {
        renderWithRouter(<Rights />);
        
        await act(async () => {
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Please enter the login!")).toBeInTheDocument();
    });

    test("shows an error if login format is incorrect", async () => {
        renderWithRouter(<Rights />);
        const loginInput = screen.getByPlaceholderText("Login");

        await act(async () => {
            await userEvent.type(loginInput, "1invalid");
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Login must start with a Latin letter")).toBeInTheDocument();
    });

    test("handles successful admin grant and resets the form", async () => {
        mockGetUserByLogin.mockResolvedValue({ user_id: "123" });
        mockGrantToAdmin.mockResolvedValue({ success: true });

        renderWithRouter(<Rights />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        await waitFor(() => {
            expect(mockGetUserByLogin).toHaveBeenCalledWith("testuser", "fakeToken");
            expect(mockGrantToAdmin).toHaveBeenCalledWith("123", "fakeToken");
        });

        expect(screen.getByText("Admin rights successfully granted!")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Login")).toHaveValue("");
    });

    test("shows an error when the user is not found", async () => {
        jest.spyOn(console, "error").mockImplementation(() => {});
        mockGetUserByLogin.mockResolvedValue(null);

        renderWithRouter(<Rights />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "unknownUser");
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("User not found!")).toBeInTheDocument();
        console.error.mockRestore();
    });

    test("shows an error when the server returns a 500 error", async () => {
        jest.spyOn(console, "error").mockImplementation(() => {});
        mockGetUserByLogin.mockRejectedValue({ response: { status: 500 } });

        renderWithRouter(<Rights />);

        await act(async () => {
            await userEvent.type(screen.getByPlaceholderText("Login"), "testuser");
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Server error occurred!")).toBeInTheDocument();
        console.error.mockRestore();
    });
});
