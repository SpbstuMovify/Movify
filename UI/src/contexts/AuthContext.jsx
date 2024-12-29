import React, { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { Outlet } from "react-router-dom";

const AuthContext = createContext();

export const AuthProvider = () => {

  const checkUserData = () => {
    const userData = localStorage.getItem("userData");
    if (userData) {
        const token = JSON.parse(userData).token;
        if (token) {
        try {
            const decodedToken = jwtDecode(token);
            const currentTime = Date.now() / 1000;
            if (decodedToken.exp <= currentTime) {
                clearUserData();
            }
        } catch (error) {
            console.error("Error decoding token:", error);
            clearUserData();
        }
        }
    }
  };

  const clearUserData = () => {
    localStorage.removeItem("userData");
  };

  useEffect(() => {
    checkUserData();
  }, []);

  return (
    <AuthContext.Provider value={{ checkUserData, clearUserData}}>
      <Outlet />
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);