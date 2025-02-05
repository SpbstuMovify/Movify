import React from 'react';
import { Outlet } from 'react-router-dom';
import '@testing-library/jest-dom'

export const useAuth = jest.fn(() => ({
    userData: {
        login: "login",
        email: "email@email.em",
        user_id: "user-id",
        role: "USER",
        token: "dummy-token"
    },
    checkUserData: jest.fn(),
    clearUserData: jest.fn(),
}));


export const AuthProvider = () => {
    return <Outlet />;
};
