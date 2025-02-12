import React from 'react';
import { Outlet } from 'react-router-dom';
import '@testing-library/jest-dom'

const mockAuth = {
    userData: {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "USER",
      token: "dummy-token"
    },
    checkUserData: jest.fn(),
    clearUserData: jest.fn(),
    setUserData: jest.fn(),
  };
  
export const useAuth = () => mockAuth;


export const AuthProvider = () => {
    return <Outlet />;
};
