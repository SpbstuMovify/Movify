import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import ConfirmationModal from "./ConfirmationModal";
import '@testing-library/jest-dom'
describe("ConfirmationModal Component", () => {
    
    test("does not render when 'opened' is false", () => {
        render(<ConfirmationModal opened={false} />);
        expect(screen.queryByText("Are you sure you want to delete your account?")).not.toBeInTheDocument();
    });

    test("renders correctly when 'opened' is true", () => {
        render(<ConfirmationModal opened={true} />);
        expect(screen.getByText("Are you sure you want to delete your account?")).toBeInTheDocument();
    });

    test("calls 'onClose' when close button (X) is clicked", () => {
        const mockOnClose = jest.fn();
        render(<ConfirmationModal opened={true} onClose={mockOnClose} />);

        fireEvent.click(screen.getByRole("button", { name: "X" }));
        expect(mockOnClose).toHaveBeenCalled();
    });

    test("calls 'onClose' when 'No' button is clicked", () => {
        const mockOnClose = jest.fn();
        render(<ConfirmationModal opened={true} onClose={mockOnClose} />);

        fireEvent.click(screen.getByRole("button", { name: "No" }));
        expect(mockOnClose).toHaveBeenCalled();
    });

    test("calls 'onConfirm' when 'Yes' button is clicked", () => {
        const mockOnConfirm = jest.fn();
        render(<ConfirmationModal opened={true} onConfirm={mockOnConfirm} />);

        fireEvent.click(screen.getByRole("button", { name: "Yes" }));
        expect(mockOnConfirm).toHaveBeenCalled();
    });

    test("displays loading indicator when 'loadingDelete' is true", () => {
        render(<ConfirmationModal opened={true} loadingDelete={true} />);
        expect(screen.getByAltText("loading")).toBeInTheDocument();
    });

    test("displays error message when 'errorMessage' is provided", () => {
        render(<ConfirmationModal opened={true} errorMessage="Deletion failed!" />);
        expect(screen.getByText("Deletion failed!")).toBeInTheDocument();
    });
});
