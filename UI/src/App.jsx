// src/App.js
import React from 'react';
import Home from './pages/Home.jsx';
import SignInScreen from './pages/SignInScreen.jsx';
import Register from './pages/Register.jsx';
import { createBrowserRouter, RouterProvider, } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext.jsx';
import './App.css'

const router = createBrowserRouter([
  {
    element: <AuthProvider />,
    children: [
      { path: "/", element: <Home /> },
      { path: "/login", element: <SignInScreen /> },
      { path: "/register", element: <Register /> },
    ]
  }
]);

const App = () => {

  return (
      <div className="app">
        <RouterProvider router={router}/>
      </div>
  );
};


export default App;
