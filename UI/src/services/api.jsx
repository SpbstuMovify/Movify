// src/services/api.js
import axios from 'axios';

export const apiBaseURL = 'http://localhost:8090';

const api = axios.create({
  baseURL: `${apiBaseURL}/v1`,
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

export const getUserByLogin = async (login, jwtToken) => {
  try {
    const response = await api.post('/users/info', {
      "login": login,
      "search_type": "LOGIN"
    }, {
      headers: {
        "Authorization": `${jwtToken}`
      }
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const getUserByEmail = async (email) => {
  try {
    const response = await api.post('/users/info', {
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

export const searchFilms = async (pageSize, pageNumber, year = null, genre = null, title = null, age_restriction = null) => {
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
    const response = await api.post(`/contents/search`, requestBody, {
      headers: {
        'Content-Type': "application/json",
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const createFilm = async (jwtToken, title="New film", quality="P1080", genre="COMEDY", category="MOVIE",
  ageRestriction="SIX_PLUS", description="Description", publisher="Publisher", cast_members=[{
    "employee_full_name": "Actor name",
    "role_name": "Role"
}], year="1900") => {
  try {
    const response = await api.post(`/contents`, {
      "title": title,
      "quality": quality,
      "genre": genre,
      "year": year,
      "category": category,
      "age_restriction": ageRestriction,
      "description": description,
      "publisher": publisher,
      "cast_members": cast_members
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

export const deleteFilm = async (jwtToken, contentId) => {
  try {
    const response = await api.delete(`/contents/${contentId}`, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const createEpisode = async (contentId, episodeNum, seasonNum, title, description, jwtToken) => {
  try {
    const response = await api.post(`/episodes`, {
      "episode_num": episodeNum,
      "season_num": seasonNum,
      "title": title,
      "description": description,
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

export const updateFilm = async (contentId, updateFields, jwtToken) => {
  try {
    const response = await api.put(`/contents/${contentId}`, updateFields, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const updateEpisode = async (episodeId, updateFields, jwtToken) => {
  try {
    const response = await api.put(`/episodes/${episodeId}`, updateFields, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const uploadImage = async (contentId, file, jwtToken) => {
  try {
    const formData = new FormData();
    formData.append("file", file);
    const response = await api.post(`/buckets/movify-videos/files?prefix=${contentId}%2F&process=false&destination=ContentImageUrl`,
      formData, {
      headers: {
        "Content-Type": "multipart/form-data",
        "Authorization": `Bearer ${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const uploadVideo = async (contentId, episodeId, file, jwtToken) => {
  try {
    const formData = new FormData();
    formData.append("file", file);
    const response = await api.post(`/buckets/movify-videos/files?prefix=${contentId}%2F${episodeId}%2F&process=true&destination=EpisodeVideoUrl`,
      formData, {
      headers: {
        "Content-Type": "multipart/form-data",
        "Authorization": `Bearer ${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const deleteEpisode = async (episodeId, jwtToken) => {
  try {
    const response = await api.delete(`/episodes/${episodeId}`, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}

export const grantToAdmin = async (userId, jwtToken) => {
  try {
    const response = await api.put(`/users/role/${userId}`, {}, {
      headers: {
        "Authorization": `${jwtToken}`
      },
    });
    return response.data;
  } catch (error) {
    throw error;
  }
}
