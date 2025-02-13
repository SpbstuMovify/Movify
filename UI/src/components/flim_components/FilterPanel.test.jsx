import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useForm } from "react-hook-form";
import FilterPanel from "./FilterPanel";
import '@testing-library/jest-dom';

function FilterPanelWrapper({ currentYear }) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = () => {};

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <FilterPanel register={register} errors={errors} currentYear={currentYear} />
      <button type="submit">Submit</button>
    </form>
  );
}

describe("FilterPanel year validation", () => {
  const currentYear = 2025;
  test("shows error when year value is greater than currentYear", async () => {
    render(<FilterPanelWrapper currentYear={currentYear} />);
    const yearInput = screen.getByLabelText("Release year:");
    
    fireEvent.change(yearInput, { target: { value: "2030" } });
    fireEvent.click(screen.getByRole("button", { name: /submit/i }));

    await waitFor(() => {
      expect(
        screen.getByText(`Enter a number between 1900 and ${currentYear}`)
      ).toBeInTheDocument();
    });
  });

  test("shows error when year value is less than 1900", async () => {
    render(<FilterPanelWrapper currentYear={currentYear} />);
    const yearInput = screen.getByLabelText("Release year:");
    
    fireEvent.change(yearInput, { target: { value: "1890" } });
    fireEvent.click(screen.getByRole("button", { name: /submit/i }));

    await waitFor(() => {
      expect(
        screen.getByText(`Enter a number between 1900 and ${currentYear}`)
      ).toBeInTheDocument();
    });
  });

  test("does not show error when year value is within valid range", async () => {
    render(<FilterPanelWrapper currentYear={currentYear} />);
    const yearInput = screen.getByLabelText("Release year:");
    
    fireEvent.change(yearInput, { target: { value: "2000" } });
    fireEvent.click(screen.getByRole("button", { name: /submit/i }));

    await waitFor(() => {
      expect(
        screen.queryByText(`Enter a number between 1900 and ${currentYear}`)
      ).not.toBeInTheDocument();
    });
  });
});
