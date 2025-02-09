import React from "react";
import { screen, fireEvent } from "@testing-library/react";
import Home from "./Home";
import '@testing-library/jest-dom';
import { renderWithRouter } from '../configs/testConfig';

jest.clearAllMocks();
jest.mock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));

// Mock useNavigate from React Router
const mockNavigate = jest.fn();
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

describe("Home Component", () => {
  test("renders Home component correctly", () => {
    renderWithRouter(<Home />);

    expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent("Unlimited movies, TV shows, and more");
    expect(screen.getByRole("heading", { level: 2 })).toHaveTextContent("Watch anywhere. Cancel at any time.");
    expect(screen.getByRole("heading", { level: 3 })).toHaveTextContent("Ready to watch?");
  });

  test("renders Navigation component", () => {
    renderWithRouter(<Home />);

    expect(screen.getByTestId("nav-logo")).toBeInTheDocument();
  });

  test("navigates to /films when 'Get Started' button is clicked", () => {
    renderWithRouter(<Home />);

    const getStartedButton = screen.getByRole("button", { name: "Get Started" });
    fireEvent.click(getStartedButton);

    expect(mockNavigate).toHaveBeenCalledWith("/films");
  });
});
