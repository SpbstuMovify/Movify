import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';

const ProtectedRoute = () => {
  const { userData, checkUserData } = useAuth();
  const navigate = useNavigate();
  useEffect(() => {
    // token verification
    checkUserData();
  }, [userData]);

  if (!userData) {
    navigate('/login');
    return;
  }

  return <Outlet />;
};

export default ProtectedRoute;
