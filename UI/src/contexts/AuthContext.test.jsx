import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { AuthProvider, useAuth } from './AuthContext';
import { jwtDecode } from 'jwt-decode';
import '@testing-library/jest-dom';

jest.mock('jwt-decode');

beforeEach(() => {
  jest.resetModules();
  jest.unmock('@contexts/AuthContext');
  jest.clearAllMocks();
  jest.restoreAllMocks();

  jest.spyOn(Storage.prototype, 'getItem').mockImplementation(() => null);
  jest.spyOn(Storage.prototype, 'setItem').mockImplementation(() => {});
  jest.spyOn(Storage.prototype, 'removeItem').mockImplementation(() => {});
});


const TestComponent = () => {
  const { checkUserData, userData, clearUserData } = useAuth();
  return (
    <div>
      <button onClick={checkUserData}>Check User</button>
      <button onClick={clearUserData}>Clear User</button>
      <div data-testid="user">{userData ? userData.login : 'No User'}</div>
    </div>
  );
};

describe('AuthContext', () => {
  test('sets userData from localStorage when button is clicked', () => {
    const fakeUserData = { token: 'fakeToken', login: 'john_doe' };
    localStorage.getItem.mockReturnValue(JSON.stringify(fakeUserData));
    jwtDecode.mockReturnValue({ exp: Date.now() / 1000 + 1000 });

    render(
      <AuthProvider useChildren={true}>
        <TestComponent />
      </AuthProvider>
    );

    fireEvent.click(screen.getByText('Check User'));

    expect(localStorage.getItem).toHaveBeenCalledWith('userData');
    expect(screen.getByTestId('user')).toHaveTextContent('john_doe');
  });

  test('clears userData if the token is expired', () => {
    const expiredToken = { token: 'expiredToken', login: 'expired_user' };
    localStorage.getItem.mockReturnValue(JSON.stringify(expiredToken));
    jwtDecode.mockReturnValue({ exp: Date.now() / 1000 - 1000 });

    render(
      <AuthProvider useChildren={true}>
        <TestComponent />
      </AuthProvider>
    );
    screen.debug();

    fireEvent.click(screen.getByText('Check User'));

    expect(screen.getByTestId('user')).toHaveTextContent('No User');
    expect(localStorage.removeItem).toHaveBeenCalledWith('userData');
  });

  test('clears userData if token decoding fails', () => {
    const invalidToken = { token: 'invalidToken', login: 'error_user' };
    localStorage.getItem.mockReturnValue(JSON.stringify(invalidToken));
    jwtDecode.mockImplementation(() => {
      throw new Error('Invalid token');
    });

    render(
      <AuthProvider useChildren={true}>
        <TestComponent />
      </AuthProvider>
    );

    fireEvent.click(screen.getByText('Check User'));

    expect(screen.getByTestId('user')).toHaveTextContent('No User');
    expect(localStorage.removeItem).toHaveBeenCalledWith('userData');
  });

  test('clears userData when Clear button is clicked', () => {
    const user = { token: 'validToken', login: 'clear_user' };
    localStorage.getItem.mockReturnValue(JSON.stringify(user));
    jwtDecode.mockReturnValue({ exp: Date.now() / 1000 + 1000 });

    render(
      <AuthProvider useChildren={true}>
        <TestComponent />
      </AuthProvider>
    );

    fireEvent.click(screen.getByText('Check User'));
    expect(screen.getByTestId('user')).toHaveTextContent('clear_user');

    fireEvent.click(screen.getByText('Clear User'));
    expect(screen.getByTestId('user')).toHaveTextContent('No User');
    expect(localStorage.removeItem).toHaveBeenCalledWith('userData');
  });

  test('clears userData if token is null', () => {
    const user = { token: null, login: 'clear_user' };
    localStorage.getItem.mockReturnValue(JSON.stringify(user));
    jwtDecode.mockReturnValue({ exp: Date.now() / 1000 + 1000 });

    render(
      <AuthProvider useChildren={true}>
        <TestComponent />
      </AuthProvider>
    );

    fireEvent.click(screen.getByText('Check User'));
    
    expect(screen.getByTestId('user')).toHaveTextContent('No User');
    expect(localStorage.removeItem).toHaveBeenCalledWith('userData');
  });
});
