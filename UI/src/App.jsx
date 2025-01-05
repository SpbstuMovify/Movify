// src/App.js
import React from 'react';
import Home from './pages/Home.jsx';
import SignInScreen from './pages/SignInScreen.jsx';
import Register from './pages/Register.jsx';
import { createBrowserRouter, RouterProvider, } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext.jsx';
import Films from './pages/Films.jsx';
import Profile from './pages/Profile.jsx';
import './App.css'
import ProtectedRoute from './components/ProtectedRoute.jsx';

const router = createBrowserRouter([
  {
    element: <AuthProvider />,
    children: [
      { path: "/", element: <Home /> },
      { path: "/login", element: <SignInScreen /> },
      { path: "/register", element: <Register /> },
      { path: "/films", element: <Films /> },
      { element: <ProtectedRoute/>,
        children: [
          {path: "/profile", element: <Profile />}
        ] 
      },
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
