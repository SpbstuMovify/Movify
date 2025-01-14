import React, { createContext, useContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import { Outlet } from "react-router-dom";

const AuthContext = createContext();

export const AuthProvider = () => {

  const [userData, setUserData] = useState();

  const checkUserData = () => {
    let localUserData = userData;
    if (!localUserData)
    {
      localUserData = JSON.parse(localStorage.getItem("userData")) 
      setUserData(localUserData);
    }
    if (localUserData) {
        const token = localUserData.token;
        if (token) {
        try {
            const decodedToken = jwtDecode(token);
            const currentTime = Date.now() / 1000;
            if (decodedToken.exp <= currentTime) {
                clearUserData();
            }
            else {
              setUserData(localUserData);
            }
        } catch (error) {
            console.error("Error decoding token:", error);
            clearUserData();
        }
        }
    }
  };

  const clearUserData = () => {
    setUserData(null);
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