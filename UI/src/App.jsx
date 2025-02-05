import React from 'react';
import { createBrowserRouter, RouterProvider, } from 'react-router-dom';
import './App.css'
import routesConfig from './configs/routesConfig.jsx';

const router = createBrowserRouter(routesConfig);

const App = () => {

  return (
    <div className="app">
      <RouterProvider router={router} />
    </div>
  );
};


export default App;
