// src/services/api.js
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:8085/v1', // Replace with your backend URL
});

export const register = async (email, password, login, firstName, lastName) => {
  try {
    const response = await api.post('/users/register', { 
      "email": email,
      "login": login,
      "password": password,
      "first_name": firstName,
      "last_name": lastName,
      "role": "USER" 
    });
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const login = async (username, password, ip) => {
  let requestBody = {};
  if (/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(username)) {
    requestBody = { 
      "email": username,
      "password": password,
      "search_type": "EMAIL"
    }
  }
  else {
    requestBody = { 
      "login": username,
      "password": password,
      "search_type": "LOGIN"
    }
  }
  try {
    const response = await api.post('/users/login', requestBody, {
      headers: {
        'ip': ip,
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getUserById = async (id) => {
  try {
    const response = await api.post('/users/info', { 
      "user_id": id,
      "search_type": "ID"
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const getUserByLogin = async (login) => {
  try {
    const response  = await api.post('/users/info', { 
      "login": login,
      "search_type": "LOGIN"
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const getUserByEmail = async (email) => {
  try {
    const response  = await api.post('/users/info', { 
      "email": email,
      "search_type": "EMAIL"
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const getFilmById = async (id) => {
  try {
    const response = await api.get(`/contents/${id}`, {});
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const getFilmPage = async (pageSize, pageNumber) => {
  try {
    const response = await api.get(`/contents?page_size=${pageSize}&page_Number=${pageNumber}`, {});
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const createFilm = async (title, quality, genre, category,
   ageRestriction, description, publisher) => { // ADD CAST MEMBERS
  try {
    const response = await api.get(`/contents`, {
      "title": title,
      "quality": quality,
      "genre": genre,
      "category": category,
      "age_restriction": ageRestriction,
      "description": description,
      "publisher": publisher
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}
