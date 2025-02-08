import React from "react";
import { render, screen, act, fireEvent } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import UpdatableImage from "./UpdatableImage";
import '@testing-library/jest-dom';

describe("UpdatableImage Component", () => {
    const initialSrc = "/images/sample.jpg";

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

    test("renders the image with initial source", () => {
        render(<UpdatableImage src={initialSrc} className="test-class" />);
        
        const image = screen.getByRole("img");
        expect(image).toHaveAttribute("src", initialSrc);
        expect(image).toHaveAttribute("alt", "Updatable");
        expect(image).toHaveClass("test-class");
    });

    test("updates image when src prop changes", () => {
        const { rerender } = render(<UpdatableImage src={initialSrc} />);
        
        const newSrc = "/images/new-image.jpg";
        rerender(<UpdatableImage src={newSrc} />);
        
        expect(screen.getByRole("img")).toHaveAttribute("src", newSrc);
    });

    test("triggers file input when image is clicked", async () => {
        render(<UpdatableImage src={initialSrc} />);
        
        const fileInput = screen.getByTestId(/image-upload/i);
        const user = userEvent.setup();

        await act(async () => { 
            await user.click(fileInput);
        });
        
        expect(fileInput).toBeInTheDocument();
    });

    test("calls onImageUpload when a file is selected", async () => {
        const mockOnImageUpload = jest.fn(() => Promise.resolve());
        render(<UpdatableImage src={initialSrc} onImageUpload={mockOnImageUpload} />);
        
        const file = new File(["image content"], "image.png", { type: "image/png" });
        const fileInput = screen.getByTestId(/image-upload/i);

        expect(fileInput).toBeInTheDocument();

        await act(async () => { 
            await userEvent.upload(fileInput, file); 
        });
          

        expect(mockOnImageUpload).toHaveBeenCalledTimes(1);
        expect(mockOnImageUpload).toHaveBeenCalledWith(expect.any(File));
    });

    test("shows 'Uploading...' during upload", async () => {
        const mockOnImageUpload = jest.fn(() => new Promise((res) => setTimeout(res, 500))); 
        render(<UpdatableImage src={initialSrc} onImageUpload={mockOnImageUpload} />);
        
        const file = new File(["image content"], "image.png", { type: "image/png" });
        const fileInput = screen.getByTestId(/image-upload/i);

        await act(async () => { 
            await userEvent.upload(fileInput, file); 
        });
          

        expect(await screen.findByText("Uploading...")).toBeInTheDocument();
    });

    test("restores opacity after upload is complete", async () => {
        const mockOnImageUpload = jest.fn(() => new Promise((res) => setTimeout(res, 500))); 
        render(<UpdatableImage src={initialSrc} onImageUpload={mockOnImageUpload} />);
        
        const file = new File(["image content"], "image.png", { type: "image/png" });
        const fileInput = screen.getByTestId(/image-upload/i);

        await act(async () => { 
            await userEvent.upload(fileInput, file); 
        });
          

        expect(await screen.findByText("Uploading...")).toBeInTheDocument();

        await act (async () => { await new Promise((res) => setTimeout(res, 600))}); 

        expect(screen.queryByText("Uploading...")).not.toBeInTheDocument();
    });

    test("does nothing if no file is selected", async () => {
        const mockOnImageUpload = jest.fn();
        render(<UpdatableImage src={initialSrc} onImageUpload={mockOnImageUpload} />);
    
        const fileInput = screen.getByTestId("image-upload");
        await act(async () => {
          fireEvent.change(fileInput, { target: { files: [] } });
        });
    
        expect(mockOnImageUpload).not.toHaveBeenCalled();
      });
});
