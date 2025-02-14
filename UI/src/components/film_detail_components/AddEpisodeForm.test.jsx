import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import AddEpisodeForm from "./AddEpisodeForm";
import "@testing-library/jest-dom";


describe("AddEpisodeForm", () => {
  const dummyRegister = jest.fn(() => ({}));
  const dummyHandleEpisodeCreate = jest.fn((e) => {
    e.preventDefault();
  });
  const dummySetShowForm = jest.fn();

  const defaultProps = {
    userData: { role: "ADMIN" },
    register: dummyRegister,
    handleEpisodeCreate: dummyHandleEpisodeCreate,
    errors: {},
    showForm: false,
    setShowForm: dummySetShowForm,
  };

  afterEach(() => {
    jest.clearAllMocks();
  });

  test("renders toggle button for admin user with 'Add Episode' text when showForm is false", () => {
    render(<AddEpisodeForm {...defaultProps} />);
    const toggleButton = screen.getByRole("button", { name: /Add Episode/i });
    expect(toggleButton).toBeInTheDocument();
  });

  test("toggle button shows 'Cancel' when showForm is true", () => {
    render(<AddEpisodeForm {...defaultProps} showForm={true} />);
    const toggleButton = screen.getByRole("button", { name: /Cancel/i });
    expect(toggleButton).toBeInTheDocument();
  });

  test("clicking toggle button calls setShowForm to toggle form visibility", () => {
    render(<AddEpisodeForm {...defaultProps} showForm={false} />);
    const toggleButton = screen.getByRole("button", { name: /Add Episode/i });
    fireEvent.click(toggleButton);
    expect(dummySetShowForm).toHaveBeenCalledTimes(1);
  });

  test("renders the form when showForm is true", () => {
    render(<AddEpisodeForm {...defaultProps} showForm={true} />);
    const form = document.querySelector(".add-episode-form");
    expect(form).toBeInTheDocument();
  });

  test("does not render the form when showForm is false", () => {
    render(<AddEpisodeForm {...defaultProps} showForm={false} />);
    const form = document.querySelector(".add-episode-form");
    expect(form).toBeNull();
  });

  test("renders error message for season number when error exists", () => {
    const errorObj = { season_num: { message: "Season number is required" } };
    render(<AddEpisodeForm {...defaultProps} showForm={true} errors={errorObj} />);
    const errorMsg = screen.getByText("Season number is required");
    expect(errorMsg).toBeInTheDocument();
  });

  test("renders error message for episode number when error exists", () => {
    const errorObj = { episode_num: { message: "Episode number is required" } };
    render(<AddEpisodeForm {...defaultProps} showForm={true} errors={errorObj} />);
    const errorMsg = screen.getByText("Episode number is required");
    expect(errorMsg).toBeInTheDocument();
  });

  test("calls handleEpisodeCreate on form submission", async () => {
    render(<AddEpisodeForm {...defaultProps} showForm={true} />);
    const form = document.querySelector(".add-episode-form");
    fireEvent.submit(form);
    await waitFor(() => {
      expect(dummyHandleEpisodeCreate).toHaveBeenCalled();
    });
  });

  test("does not render toggle button for non-admin user", () => {
    render(<AddEpisodeForm {...defaultProps} userData={{ role: "USER" }} />);
    const toggleButton = screen.queryByRole("button", { name: /Add Episode/i });
    expect(toggleButton).toBeNull();
  });

  test("calls register for season_num and episode_num inputs", () => {
    render(<AddEpisodeForm {...defaultProps} showForm={true} />);
    expect(dummyRegister).toHaveBeenCalledWith("season_num", expect.any(Object));
    expect(dummyRegister).toHaveBeenCalledWith("episode_num", expect.any(Object));
  });

  test("toggle button calls setShowForm with a callback that toggles the value", () => {
    const fakeSetShowForm = jest.fn((toggleFn) => {
      return toggleFn(false);
    });
  
    render(
      <AddEpisodeForm
        userData={{ role: "ADMIN" }}
        register={() => ({})}
        handleEpisodeCreate={jest.fn()}
        errors={{}}
        showForm={false}
        setShowForm={fakeSetShowForm}
      />
    );
  
    const toggleButton = screen.getByRole("button", { name: /Add Episode/i });
    fireEvent.click(toggleButton);
  
    expect(fakeSetShowForm).toHaveBeenCalled();
    const toggleFn = fakeSetShowForm.mock.calls[0][0];
    expect(typeof toggleFn).toBe("function");
  
    expect(toggleFn(true)).toBe(false);
    expect(toggleFn(false)).toBe(true);
  });
});
