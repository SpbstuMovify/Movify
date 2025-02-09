import React from "react";
import { render, screen, fireEvent, waitFor, act } from "@testing-library/react";
import { useForm } from "react-hook-form";
import ChangePasswordForm from "./ChangePasswordForm";
import '@testing-library/jest-dom'

const Wrapper = ({ handlePasswordChange }) => {
    const formMethods = useForm();
    return <ChangePasswordForm {...formMethods} 
        errors={formMethods.formState.errors}
        isSubmitSuccessful={formMethods.formState.isSubmitSuccessful}
        isSubmitting={formMethods.formState.isSubmitting} 
        handlePasswordChange={handlePasswordChange} />;
};

describe("ChangePasswordForm Component", () => {
    const mockHandlePasswordChange = jest.fn();

    const setup = () => {
        return render(<Wrapper handlePasswordChange={mockHandlePasswordChange} />);
    };

    test("renders form inputs correctly", () => {
        setup();
        expect(screen.getByPlaceholderText("Enter the password")).toBeInTheDocument();
        expect(screen.getByPlaceholderText("Repeat the password")).toBeInTheDocument();
        expect(screen.getByRole("button", { name: "Submit" })).toBeInTheDocument();
    });

    test("allows user to type in password fields", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const repeatPasswordInput = screen.getByPlaceholderText("Repeat the password");

        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "Password1!" } });
            fireEvent.change(repeatPasswordInput, { target: { value: "Password1!" } });
        });

        expect(passwordInput).toHaveValue("Password1!");
        expect(repeatPasswordInput).toHaveValue("Password1!");
    });

    test("shows error when password is missing", async () => {
        setup();
        await act(async () => { 
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Please enter the password!")).toBeInTheDocument();
    });

    test("validates password including at least one number", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        
        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "abcdef" }});
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Password should include at least one number")).toBeInTheDocument();
    });

    test("validates password strength at least one special symbol", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        
        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "abcdef1" }});
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Password should include at least one special symbol")).toBeInTheDocument();
    });

    test("validates password including at least one lowercase latin letter", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        
        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "QQQQQQ@1Q" }});
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Password should include at least one lowercase latin letter")).toBeInTheDocument();
    });

    test("validates password including at least one uppercase latin letter", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        
        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "qqqqqq@1q" }});
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Password should include at least one uppercase latin letter")).toBeInTheDocument();
    });

    test("shows error when repeated password does not match", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const repeatPasswordInput = screen.getByPlaceholderText("Repeat the password");

        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "Password1!" } });
            fireEvent.change(repeatPasswordInput, { target: { value: "Mismatch!" } });

            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        expect(await screen.findByText("Passwords do not match")).toBeInTheDocument();
    });

    test("submits form successfully when inputs are valid", async () => {
        setup();
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const repeatPasswordInput = screen.getByPlaceholderText("Repeat the password");

        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "Password1!" } });
            fireEvent.change(repeatPasswordInput, { target: { value: "Password1!" } });

            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        await waitFor(() => {
            expect(mockHandlePasswordChange).toHaveBeenCalled();
        });
    });

    test("displays success message when password is changed", async () => {
        setup();
        
        const passwordInput = screen.getByPlaceholderText("Enter the password");
        const repeatPasswordInput = screen.getByPlaceholderText("Repeat the password");
        
        await act(async () => { 
            fireEvent.change(passwordInput, { target: { value: "Password1!" } });
            fireEvent.change(repeatPasswordInput, { target: { value: "Password1!" } });
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });

        await waitFor(() => {
            expect(screen.getByText("Password was changed!")).toBeInTheDocument();
        });
    });

    test("displays loading indicator when submitting", async () => {
        const mockSetError = jest.fn();
        const mockHandlePasswordChange = jest.fn();
    
        let isSubmitting = true;
    
        render(
            <ChangePasswordForm
                register={jest.fn()}
                handleSubmit={(fn) => (e) => {
                    e.preventDefault();
                    isSubmitting = true;
                    return new Promise((resolve) => {
                        setTimeout(() => {
                            fn({}, { setError: mockSetError });
                            isSubmitting = false;
                            resolve();
                        }, 1000);
                    });
                }}
                handlePasswordChange={mockHandlePasswordChange}
                errors={{ root: { message: "There was an error updating your password" } }}
                isSubmitting={isSubmitting}
                isSubmitSuccessful={false}
                getValues={jest.fn()}
                setError={jest.fn()}
            />
        );
    
        await act(async () => { 
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });
    
        await waitFor(() => {
            expect(screen.getByAltText("loading")).toBeInTheDocument();
        }, { timeout: 1500 }); 
    });
    

    test("displays error message when submission fails", async () => {
        const mockSetError = jest.fn();

        const mockHandlePasswordChange = jest.fn(async (_, { setError }) => {
            setError("root", { message: "There was an error updating your password" });
        });
    
        render(
            <ChangePasswordForm
                register={jest.fn()}
                handleSubmit={(fn) => (e) => { e.preventDefault(); return fn({}, { setError: mockSetError }); }}
                handlePasswordChange={mockHandlePasswordChange}
                errors={{ root: { message: "There was an error updating your password" } }}
                isSubmitting={false}
                isSubmitSuccessful={false}
                getValues={jest.fn()}
                setError={mockSetError}
            />
        );
    
        await act(async () => { 
            fireEvent.click(screen.getByRole("button", { name: "Submit" }));
        });
    
        await waitFor(() => {
            expect(mockSetError).toHaveBeenCalledWith("root", { message: "There was an error updating your password" });
        });
    
        expect(await screen.findByText("There was an error updating your password")).toBeInTheDocument();
    });
    
    
    
});
