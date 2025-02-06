
import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import DropdownMultiSelect from "./DropdownMultiSelect"; 
import '@testing-library/jest-dom';

describe("DropdownMultiSelect", () => {
  const mockOnChange = jest.fn();
  const options = ["Option 1", "Option 2", "Option 3"];

  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders the dropdown component correctly", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    expect(screen.getByRole("button")).toBeInTheDocument();
    expect(screen.getByText("Select options")).toBeInTheDocument();
  });

  test("opens dropdown when clicking the button", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    const button = screen.getByRole("button");
    fireEvent.click(button);
    
    options.forEach((option) => {
      expect(screen.getByLabelText(option)).toBeInTheDocument();
    });
  });

  test("closes dropdown when clicking outside", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    const button = screen.getByRole("button");
    fireEvent.click(button);
    
    expect(screen.getByLabelText("Option 1")).toBeInTheDocument(); 
    
    fireEvent.click(document.body);
    
    expect(screen.queryByLabelText("Option 1")).not.toBeInTheDocument(); 
  });

  test("selects and deselects options", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    fireEvent.click(screen.getByRole("button"));
    
    const option1 = screen.getByLabelText("Option 1");
    fireEvent.click(option1);
    expect(option1.checked).toBe(true);
    expect(mockOnChange).toHaveBeenCalledWith(["Option 1"]);
    
    fireEvent.click(option1);
    expect(option1.checked).toBe(false);
    expect(mockOnChange).toHaveBeenCalledWith([]);
  });

  test("displays selected options in the button", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    fireEvent.click(screen.getByRole("button"));
    
    fireEvent.click(screen.getByLabelText("Option 1"));
    fireEvent.click(screen.getByLabelText("Option 2"));
    
    expect(screen.getByText("Option 1, Option 2")).toBeInTheDocument();
  });

  test("hides dropdown when clicking again on the button", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    const button = screen.getByRole("button");
    fireEvent.click(button); 
    expect(screen.getByLabelText("Option 1")).toBeInTheDocument();
    
    fireEvent.click(button); 
    expect(screen.queryByLabelText("Option 1")).not.toBeInTheDocument();
  });

  test("input field contains the correct selected values", () => {
    render(<DropdownMultiSelect name="test" options={options} onChange={mockOnChange} />);
    
    fireEvent.click(screen.getByRole("button"));
    
    fireEvent.click(screen.getByLabelText("Option 1"));
    fireEvent.click(screen.getByLabelText("Option 2"));
    
    const hiddenInput = screen.getByDisplayValue("Option 1,Option 2");
    expect(hiddenInput).toBeInTheDocument();
  });
});
