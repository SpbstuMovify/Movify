import React from "react";
import { render, screen } from "@testing-library/react";
import SearchForm from "./SearchForm";
import '@testing-library/jest-dom';

test("renders error message when errors.root exists", () => {
  const dummyRegister = jest.fn();
  const dummyHandleSubmit = (fn) => fn;
  const dummyOnSubmit = jest.fn();
  const errors = { root: { message: "Test error message" } };

  render(
    <SearchForm
      register={dummyRegister}
      handleSubmit={dummyHandleSubmit}
      onSubmit={dummyOnSubmit}
      errors={errors}
    />
  );

  expect(screen.getByText("Test error message")).toBeInTheDocument();
});
