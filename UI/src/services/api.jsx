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

export const getUserById = async (id, jwtToken) => {
  try {
    const response = await api.post('/users/info', { 
      "user_id": id,
      "search_type": "ID"
    }, {
      headers: {
        "Authorization": `${jwtToken}`
      },
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
    const response = await api.get(`/contents/${id}`);
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const searchFilms = async (pageSize, pageNumber, year=null, genre=null, title=null, age_restriction=null) => {
  try {
    const requestBody = JSON.stringify({
      "title": title,
      "year": year,
      "genre": genre,
      "age_restriction": age_restriction,
      "page_size": pageSize,
      "page_number": pageNumber,
    }, (key, value) => {
      return value === null || value === "" ? undefined : value;
  });
    const response = await api.post(`http://localhost:8085/v1/contents/search`, requestBody, {
      headers: {
        'Content-Type': "application/json",
      },
    });
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

export const getPersonalList = async (userId, jwtToken) => {
 try {
   const response = await api.get(`/users/personal-list/${userId}`, {
    headers: {
      "Authorization": `${jwtToken}`
    },
  });
   return response.data;
 } catch (error) {
   throw error;
 }
}

export const addToPersonalList = async (userId, contentId, jwtToken) => {
  try {
    const response = await api.post(`/users/personal-list`, {
      "user_id": userId,
      "content_id": contentId
    }, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
 }

 export const removeFromPersonalList = async (userId, contentId, jwtToken) => {
  try {
    const response = await api.delete(`/users/personal-list`, {
      data: {
        "user_id": userId,
        "content_id": contentId
      },
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
 }

 export const changePassword = async (password, login, email, role, jwtToken) => {
  try {
    const response = await api.post(`/users/password-recovery`, {
      "password": password,
      "email": email,
      "login": login,
      "role": role,
    }, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
 }

 export const deleteUser = async (userId, jwtToken) => {
  try {
    const response = await api.delete(`/users/${userId}`, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
 }

 export const getEpisodes = async (contentId) => {
  try {
    const response = await api.get(`/episodes?content_id=${contentId}`);
    return response.data;
  } catch (error) {
    throw error;
  }
 }