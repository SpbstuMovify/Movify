import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { useAuth } from '../../contexts/AuthContext';

const ProtectedRoute = () => {
  const { userData, checkUserData } = useAuth();
  const navigate = useNavigate();
  useEffect(() => {
    checkUserData();
    if (!userData) {
      navigate('/login');
      return;
    }
  }, [userData]);

  return <Outlet />;
};

export default ProtectedRoute;
