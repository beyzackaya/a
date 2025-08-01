import React from 'react';
import { Navigate } from 'react-router-dom';
import { authservice } from './services/authservice';

export default function PrivateRoute({ children }) {
  const token = authservice.getToken();
  return token ? children : <Navigate to="/" />;
}