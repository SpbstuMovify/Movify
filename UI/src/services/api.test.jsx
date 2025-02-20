const mockAxiosInstance = {
    get: jest.fn(),
    post: jest.fn(),
    put: jest.fn(),
    delete: jest.fn(),
  };
  
jest.mock("axios", () => ({
    create: () => ({
        get: (...args) => mockAxiosInstance.get(...args),
        post: (...args) => mockAxiosInstance.post(...args),
        put: (...args) => mockAxiosInstance.put(...args),
        delete: (...args) => mockAxiosInstance.delete(...args),
    }),
}));
  
  import axios from "axios";
  import {
    apiBaseURL,
    register,
    login,
    getUserById,
    getUserByLogin,
    getUserByEmail,
    getFilmById,
    searchFilms,
    createFilm,
    deleteFilm,
    createEpisode,
    getPersonalList,
    addToPersonalList,
    removeFromPersonalList,
    changePassword,
    deleteUser,
    getEpisodes,
    updateFilm,
    updateEpisode,
    uploadImage,
    uploadVideo,
    deleteEpisode,
    grantToAdmin,
  } from "./api";
  
describe("API service functions", () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test("register sends correct payload", async () => {
        mockAxiosInstance.post.mockResolvedValue({ data: { registered: true } });
        const result = await register("test@test.com", "pass123", "testuser", "First", "Last");
        expect(mockAxiosInstance.post).toHaveBeenCalledWith(
        "/users/register",
        {
            email: "test@test.com",
            login: "testuser",
            password: "pass123",
            first_name: "First",
            last_name: "Last",
            role: "USER",
        }
        );
        expect(result).toEqual({ registered: true });
    });
  test("login sends correct payload for email login", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { token: "abc" } });
    const result = await login("test@test.com", "pass123", "127.0.0.1");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      "/users/login",
      {
        email: "test@test.com",
        password: "pass123",
        search_type: "EMAIL",
      },
      { headers: { ip: "127.0.0.1" } }
    );
    expect(result).toEqual({ token: "abc" });
  });

  test("login sends correct payload for non-email login", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { token: "def" } });
    const result = await login("username", "pass123", "127.0.0.1");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      "/users/login",
      {
        login: "username",
        password: "pass123",
        search_type: "LOGIN",
      },
      { headers: { ip: "127.0.0.1" } }
    );
    expect(result).toEqual({ token: "def" });
  });

  test("getUserById sends correct payload and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { user: "info" } });
    const result = await getUserById("user-1", "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      "/users/info",
      { user_id: "user-1", search_type: "ID" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ user: "info" });
  });

  test("getUserByLogin sends correct payload and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { user: "info" } });
    const result = await getUserByLogin("loginName", "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      "/users/info",
      { login: "loginName", search_type: "LOGIN" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ user: "info" });
  });

  test("getUserByEmail sends correct payload", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { user: "info" } });
    const result = await getUserByEmail("test@test.com");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      "/users/info",
      { email: "test@test.com", search_type: "EMAIL" }
    );
    expect(result).toEqual({ user: "info" });
  });

  test("getFilmById sends GET request to correct URL", async () => {
    mockAxiosInstance.get.mockResolvedValue({ data: { film: "info" } });
    const result = await getFilmById("film-1");
    expect(mockAxiosInstance.get).toHaveBeenCalledWith(`/contents/film-1`);
    expect(result).toEqual({ film: "info" });
  });

  const searchFilmScenarios = [
    {
      description: "full args",
      searchScenario: async () => await searchFilms(10, 2, 2000, "COMEDY", "Test", "SIXTEEN_PLUS"),
      expectedJSON: {
        title: "Test",
        year: 2000,
        genre: "COMEDY",
        age_restriction: "SIXTEEN_PLUS",
        page_size: 10,
        page_number: 2,
      }
    },
    {
      description: "no default args",
      searchScenario: async () => await searchFilms(10, 2),
      expectedJSON: {
          title: null,
          year: null,
          genre: null,
          age_restriction: null,
          page_size: 10,
          page_number: 2,
      }
    }
  ]

  test.each(searchFilmScenarios)("searchFilms sends correct JSON payload and header $description", 
    async ({ searchScenario, expectedJSON }) => {
    mockAxiosInstance.post.mockResolvedValue({ data: { results: [] } });
    const result = await searchScenario();
    const expectedRequest = JSON.stringify(
        expectedJSON,
      (key, value) => (value === null || value === "" ? undefined : value)
    );
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/contents/search`,
      expectedRequest,
      { headers: { "Content-Type": "application/json" } }
    );
    expect(result).toEqual({ results: [] });
  });


  const createFilmScenarios = [
    {
      description: "full args",
      createScenario: async () => await createFilm(
        "jwt-token",
        "title",
        "P720",
        "ACTION",
        "SERIAL",
        "SIXTEEN_PLUS",
        "Test description",
        "Test publisher",
        [{ employee_full_name: "Employee", role_name: "Character" }],
        "2000"
      ),
      expectedBody: {
        title: "title",
        quality: "P720",
        genre: "ACTION",
        year: "2000",
        category: "SERIAL",
        age_restriction: "SIXTEEN_PLUS",
        description: "Test description",
        publisher: "Test publisher",
        cast_members: [{ employee_full_name: "Employee", role_name: "Character" }],
      },
      expectedHeaders: { headers: { Authorization: "jwt-token" } }
    },
    {
      description: "no default args",
      createScenario: async () => await createFilm("jwt-token"),
      expectedBody: {
        title: "New film",
        quality: "P1080",
        genre: "COMEDY",
        year: "1900",
        category: "MOVIE",
        age_restriction: "SIX_PLUS",
        description: "Description",
        publisher: "Publisher",
        cast_members: [{ employee_full_name: "Actor name", role_name: "Role" }],
      },
      expectedHeaders: { headers: { Authorization: "jwt-token" } }
    }
  ]

  test.each(createFilmScenarios)("createFilm sends correct payload and header $description",
    async ({ createScenario, expectedBody, expectedHeaders }) => {
    mockAxiosInstance.post.mockResolvedValue({ data: { created: true } });
    const result = await createScenario();
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/contents`,
      expectedBody,
      expectedHeaders
    );
    expect(result).toEqual({ created: true });
  });

  test("deleteFilm sends DELETE request with correct header", async () => {
    mockAxiosInstance.delete.mockResolvedValue({ data: { deleted: true } });
    const result = await deleteFilm("jwt-token", "content-1");
    expect(mockAxiosInstance.delete).toHaveBeenCalledWith(
      `/contents/content-1`,
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ deleted: true });
  });

  test("createEpisode sends correct payload and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { created: true } });
    const result = await createEpisode("content-1", 3, 1, "Episode Title", "Desc", "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/episodes`,
      {
        episode_num: 3,
        season_num: 1,
        title: "Episode Title",
        description: "Desc",
        content_id: "content-1",
      },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ created: true });
  });

  test("getPersonalList sends correct GET request with header", async () => {
    mockAxiosInstance.get.mockResolvedValue({ data: { list: [] } });
    const result = await getPersonalList("user-1", "jwt-token");
    expect(mockAxiosInstance.get).toHaveBeenCalledWith(
      `/users/personal-list/user-1`,
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ list: [] });
  });

  test("addToPersonalList sends correct payload and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { added: true, content_id: "content-1" } });
    const result = await addToPersonalList("user-1", "content-1", "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/users/personal-list`,
      { user_id: "user-1", content_id: "content-1" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ added: true, content_id: "content-1" });
  });

  test("removeFromPersonalList sends correct DELETE request with payload and header", async () => {
    mockAxiosInstance.delete.mockResolvedValue({ data: { removed: true } });
    const result = await removeFromPersonalList("user-1", "content-1", "jwt-token");
    expect(mockAxiosInstance.delete).toHaveBeenCalledWith(
      `/users/personal-list`,
      {
        data: { user_id: "user-1", content_id: "content-1" },
        headers: { Authorization: "jwt-token" },
      }
    );
    expect(result).toEqual({ removed: true });
  });

  test("changePassword sends correct payload and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { changed: true } });
    const result = await changePassword("newPass", "login", "test@test.com", "USER", "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/users/password-recovery`,
      { password: "newPass", email: "test@test.com", login: "login", role: "USER" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ changed: true });
  });

  test("deleteUser sends correct DELETE request with header", async () => {
    mockAxiosInstance.delete.mockResolvedValue({ data: { deleted: true } });
    const result = await deleteUser("user-1", "jwt-token");
    expect(mockAxiosInstance.delete).toHaveBeenCalledWith(
      `/users/user-1`,
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ deleted: true });
  });

  test("getEpisodes sends correct GET request with query param", async () => {
    mockAxiosInstance.get.mockResolvedValue({ data: { episodes: [] } });
    const result = await getEpisodes("content-1");
    expect(mockAxiosInstance.get).toHaveBeenCalledWith(`/episodes?content_id=content-1`);
    expect(result).toEqual({ episodes: [] });
  });

  test("updateFilm sends correct PUT request with payload and header", async () => {
    mockAxiosInstance.put.mockResolvedValue({ data: { updated: true } });
    const result = await updateFilm("content-1", { title: "New Title" }, "jwt-token");
    expect(mockAxiosInstance.put).toHaveBeenCalledWith(
      `/contents/content-1`,
      { title: "New Title" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ updated: true });
  });

  test("updateEpisode sends correct PUT request with payload and header", async () => {
    mockAxiosInstance.put.mockResolvedValue({ data: { updated: true } });
    const result = await updateEpisode("episode-1", { title: "Updated Episode" }, "jwt-token");
    expect(mockAxiosInstance.put).toHaveBeenCalledWith(
      `/episodes/episode-1`,
      { title: "Updated Episode" },
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ updated: true });
  });

  test("uploadImage sends correct POST request with FormData and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { uploaded: true } });
    const fakeFile = new File(["dummy content"], "image.png", { type: "image/png" });
    const result = await uploadImage("content-1", fakeFile, "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/buckets/movify-videos/files?prefix=content-1%2F&process=false&destination=ContentImageUrl`,
      expect.any(FormData),
      {
        headers: {
          "Content-Type": "multipart/form-data",
          Authorization: `Bearer jwt-token`,
        },
      }
    );
    expect(result).toEqual({ uploaded: true });
  });

  test("uploadVideo sends correct POST request with FormData and header", async () => {
    mockAxiosInstance.post.mockResolvedValue({ data: { uploaded: true } });
    const fakeFile = new File(["dummy content"], "video.mp4", { type: "video/mp4" });
    const result = await uploadVideo("content-1", "episode-1", fakeFile, "jwt-token");
    expect(mockAxiosInstance.post).toHaveBeenCalledWith(
      `/buckets/movify-videos/files?prefix=content-1%2Fepisode-1%2F&process=true&destination=EpisodeVideoUrl`,
      expect.any(FormData),
      {
        headers: {
          "Content-Type": "multipart/form-data",
          Authorization: `Bearer jwt-token`,
        },
      }
    );
    expect(result).toEqual({ uploaded: true });
  });

  test("deleteEpisode sends correct DELETE request with header", async () => {
    mockAxiosInstance.delete.mockResolvedValue({ data: { deleted: true } });
    const result = await deleteEpisode("episode-1", "jwt-token");
    expect(mockAxiosInstance.delete).toHaveBeenCalledWith(
      `/episodes/episode-1`,
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ deleted: true });
  });

  test("grantToAdmin sends correct PUT request with header", async () => {
    mockAxiosInstance.put.mockResolvedValue({ data: { granted: true } });
    const result = await grantToAdmin("user-1", "jwt-token");
    expect(mockAxiosInstance.put).toHaveBeenCalledWith(
      `/users/role/user-1`,
      {},
      { headers: { Authorization: "jwt-token" } }
    );
    expect(result).toEqual({ granted: true });
  });
});
