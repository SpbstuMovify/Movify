import React, { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { Outlet } from "react-router-dom";

const AuthContext = createContext();

export const AuthProvider = () => {

  const [userData, setUserData] = useState();

  const checkUserData = () => {
    if (!userData)
    {
      setUserData(JSON.parse(localStorage.getItem("userData")));
    }
    if (userData) {
        const token = userData.token;
        if (token) {
        try {
            const decodedToken = jwtDecode(token);
            const currentTime = Date.now() / 1000;
            if (decodedToken.exp <= currentTime) {
                clearUserData();
            }
            else {
              setUserData(userData);
            }
        } catch (error) {
            console.error("Error decoding token:", error);
            clearUserData();
        }
        }
    }
    console.log("UserData checked!")
  };

  const clearUserData = () => {
    setUserData({});
    localStorage.removeItem("userData");
  };

  useEffect(() => {
    checkUserData();
  }, []);

  return (
    <AuthContext.Provider value={{ checkUserData, clearUserData, setUserData, userData}}>
      <Outlet />
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);