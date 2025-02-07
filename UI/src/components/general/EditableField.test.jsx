import React from "react";
import { render, screen, fireEvent, cleanup } from "@testing-library/react";
import EditableField from "./EditableField";
import '@testing-library/jest-dom';

describe("EditableField", () => {
  const onChangeMock = jest.fn();
  const onSaveMock = jest.fn();
  const defaultProps = {
    label: "Name",
    value: "John Doe",
    onChange: onChangeMock,
    onSave: onSaveMock,
    disableEditButton: false,
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders correctly in read-only mode", () => {
    render(<EditableField {...defaultProps} />);
    
    expect(screen.getByText("Name")).toBeInTheDocument();
    expect(screen.getByText("John Doe")).toBeInTheDocument();
    expect(screen.getByRole("button")).toBeInTheDocument();
  });

  test("allows editing when edit button is clicked", () => {
    render(<EditableField {...defaultProps} />);
    
    fireEvent.click(screen.getByRole("button"));
    
    expect(screen.getByRole("textbox")).toBeInTheDocument();
  });

  test("calls onChange when input value changes (type = input)", () => {
    render(<EditableField {...defaultProps} />);
    
    fireEvent.click(screen.getByRole("button"));
    fireEvent.change(screen.getByRole("textbox"), { target: { value: "Jane Doe" } });

    expect(onChangeMock).toHaveBeenCalledWith("Jane Doe");
  });

  test("calls onChange when input value changes (type = textarea)", () => {
    render(<EditableField {...defaultProps} inputType="textarea" value="1231231dasdas" />);
    
    fireEvent.click(screen.getByRole("button"));
    fireEvent.change(screen.getByRole("textbox"), { target: { value: "Jane Doe" } });

    expect(onChangeMock).toHaveBeenCalledWith("Jane Doe");
  });

  test("calls onChange when input value changes (type = number)", () => {
    render(<EditableField {...defaultProps} inputType="number" value={10} />);
    
    fireEvent.click(screen.getByRole("button"));
    fireEvent.change(screen.getByRole("spinbutton"), { target: { value: 11 } });

    expect(onChangeMock).toHaveBeenCalledWith("11");
  });

  test("calls onChange when input value changes (type = select)", () => {
    render(<EditableField {...defaultProps} 
      inputType="select" 
      options={[
        { label: "Option 1", value: "1" },
        { label: "Option 2", value: "2" },
      ]}
      value="1" />);
    
    fireEvent.click(screen.getByRole("button"));
    fireEvent.change(screen.getByRole("combobox"), { target: { value: "1" } });

    expect(onChangeMock).toHaveBeenCalledWith("1");
  });

  test("calls onSave when clicking save button", () => {
    render(<EditableField {...defaultProps} />);
    
    fireEvent.click(screen.getByRole("button"));
    fireEvent.click(screen.getByRole("img").closest("button"));

    expect(onSaveMock).toHaveBeenCalledWith("John Doe");
  });

  test("throws error on invalid input type", () => {
    const consoleErrorMock = jest.spyOn(console, "error").mockImplementation(() => {});
    expect(() => {
      render(<EditableField {...defaultProps} inputType="invalid-type" />);
      fireEvent.click(screen.getByRole("button"));
    }).toThrowError("Invalid input type");
    consoleErrorMock.mockRestore();
  });

  test("renders different input types correctly", () => {
    render(<EditableField {...defaultProps} inputType="number" value={10} />);
    fireEvent.click(screen.getByRole("button"));
    expect(screen.getByRole("spinbutton")).toBeInTheDocument();
    cleanup();

    render(<EditableField {...defaultProps} inputType="textarea" />);
    fireEvent.click(screen.getByRole("button"));
    expect(screen.getByRole("textbox")).toBeInTheDocument();
    cleanup();
    
    render(
      <EditableField
        {...defaultProps}
        inputType="select"
        options={[
          { label: "Option 1", value: "1" },
          { label: "Option 2", value: "2" },
        ]}
        value="1"
      />
    );
    fireEvent.click(screen.getByRole("button"));
    expect(screen.getByRole("combobox")).toBeInTheDocument();
  });

  test("disables edit button when disableEditButton is true", () => {
    render(<EditableField {...defaultProps} disableEditButton={true} />);
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  const maxInputSize = 50;

  test("input size matches value length when below maxInputSize (text input)", () => {
    const testValue = "Hello";
    render(<EditableField disableEditButton={false} inputType="input" value={testValue} onChange={() => {}} />);
    fireEvent.click(screen.getByRole("button"));
    const input = screen.getByRole("textbox");
    
    expect(input).toHaveAttribute("size", testValue.length.toString());
  });

  test("input size is limited to maxInputSize when value length exceeds limit (text input)", () => {
    const longValue = "A".repeat(maxInputSize + 10);
    render(<EditableField disableEditButton={false} inputType="input" value={longValue} onChange={() => {}} />);
    fireEvent.click(screen.getByRole("button"));
    const input = screen.getByRole("textbox");

    expect(input).toHaveAttribute("size", maxInputSize.toString());
  });

  test("number input size matches value length when below maxInputSize", () => {
    const testValue = "12345";
    render(<EditableField disableEditButton={false} inputType="number" value={testValue} onChange={() => {}} />);
    fireEvent.click(screen.getByRole("button"));
    const input = screen.getByRole("spinbutton"); // Number inputs use 'spinbutton' role
    
    expect(input).toHaveAttribute("size", testValue.length.toString());
  });

  test("number input size is limited to maxInputSize when value length exceeds limit", () => {
    const longNumber = "9".repeat(maxInputSize + 5); // 5 characters longer than the max
    render(<EditableField disableEditButton={false} inputType="number" value={longNumber} onChange={() => {}} />);
    fireEvent.click(screen.getByRole("button"));
    const input = screen.getByRole("spinbutton");

    expect(input).toHaveAttribute("size", maxInputSize.toString());
  });

});
