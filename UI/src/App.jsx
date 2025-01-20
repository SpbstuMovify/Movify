// src/App.js
import React from 'react';
import { createBrowserRouter, RouterProvider, } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext.jsx';
import './App.css'
import Home from './pages/Home.jsx';
import SignInScreen from './pages/SignInScreen.jsx';
import Register from './pages/Register.jsx';
import Films from './pages/Films.jsx';
import FavoriteFilms from './pages/FavoriteFilms.jsx'; 
import Profile from './pages/Profile.jsx';
import ProtectedRoute from './components/ProtectedRoute.jsx';
import FilmDetail from './pages/FilmDetail.jsx';

const router = createBrowserRouter([
  {
    element: <AuthProvider />,
    children: [
      { path: "/", element: <Home /> },
      { path: "/login", element: <SignInScreen /> },
      { path: "/register", element: <Register /> },
      { path: "/films", element: <Films /> },
      { path: "/films/:contentId", element: <FilmDetail /> },
      { element: <ProtectedRoute/>,
        children: [
          {path: "/profile", element: <Profile />},
          {path: "/favorites", element: <FavoriteFilms />}
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
