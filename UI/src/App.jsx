// src/App.js
import React from 'react';
import Home from './pages/Home.jsx';
import { createBrowserRouter, RouterProvider, } from 'react-router-dom';
import './App.css'

const router = createBrowserRouter([
  {
    path: "/", element: <Home /> },
]);

const App = () => {
  return (
      <div className="app">
        <RouterProvider router={router}/>
      </div>
  );
};


export default App;
