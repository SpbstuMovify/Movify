import React from "react";
import { act, screen, fireEvent, waitFor, within, cleanup } from "@testing-library/react";
import userEvent from '@testing-library/user-event';
import FilmDetail from "./FilmDetail";
import "@testing-library/jest-dom";
import { renderWithRouter } from "../configs/testConfig";
import mappingJSON from "../configs/config";
import * as AuthContext from "../contexts/AuthContext";

import {
  apiBaseURL,
  getFilmById,
  getEpisodes,
  getPersonalList,
  addToPersonalList,
  removeFromPersonalList,
  updateFilm,
  uploadImage,
  uploadVideo,
  updateEpisode,
  deleteEpisode
} from "../services/api";

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useParams: () => ({ contentId: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a" }),
  useNavigate: () => jest.fn(),
}));

jest.mock("react-player", () => (props) => (
  <div data-testid="react-player">{props.url}</div>
));

jest.mock("@contexts/AuthContext", () => require('../__mocks__/contexts/AuthContext'));

jest.mock('@services/api', () => require('../__mocks__/services/api'));

describe("FilmDetail", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders film details after fetching data", async () => {
    renderWithRouter(<FilmDetail />);
  
    await waitFor(() => {
      expect(screen.getByText(/Фоллаут/)).toBeInTheDocument();
    });

    expect(getFilmById).toHaveBeenCalledWith("3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a");
    expect(getEpisodes).toHaveBeenCalledWith("3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a");
  });

  test("shows remove-from-favorites button when film is in the personal list and calls remove API on click", async () => {

    const userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "USER",
      token: "dummy-token",
    };

    renderWithRouter(<FilmDetail />);

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: /Remove from favorites/i })
      ).toBeInTheDocument();
    });

    userEvent.click(
      screen.getByRole("button", { name: /Remove from favorites/i })
    );

    await waitFor(() => {
      expect(removeFromPersonalList).toHaveBeenCalledWith(
        userData.user_id,
        "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
        userData.token
      );
    });
  });

  test("shows add-to-favorites button when film is not in the personal list and calls add API on click", async () => {
    getPersonalList.mockResolvedValueOnce([]);

    await act(async () => {
      renderWithRouter(<FilmDetail />);
    });

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: /Add to favorites/i })
      ).toBeInTheDocument();
    });

    await act(async ()=>{await userEvent.click(
      screen.getByRole("button", { name: /Add to favorites/i })
    );})
    

    await waitFor(() => {
      expect(addToPersonalList).toHaveBeenCalledWith(
        "68b1ba18-b97c-4481-97d5-debaf9616182",
        "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
        "dummy-token"
      );
    });
  });

  test("allows admin to toggle the add episode form", async () => {

    AuthContext.useAuth().userData = {
        login: "login",
        email: "email@email.em",
        user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
        role: "ADMIN",
        token: "dummy-token"
    };

    await act(async () => {
      renderWithRouter(<FilmDetail />);
    });

    await waitFor(() => {
      expect(screen.getByText(/Фоллаут/i)).toBeInTheDocument();
    });

    const addEpisodeToggle = screen.getByRole("button", {
      name: /Add Episode/i,
    });
    expect(addEpisodeToggle).toBeInTheDocument();

    userEvent.click(addEpisodeToggle);

    await waitFor(() => {
      expect(screen.getByLabelText(/Season Number:/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Episode Number:/i)).toBeInTheDocument();
    });
  });

  test("selecting a season and episode displays video player/admin controls for an uploaded episode", async () => {
    const { container } = renderWithRouter(<FilmDetail />);

    await waitFor(() => {
      expect(screen.getByText(/Фоллаут/i)).toBeInTheDocument();
    });

    const selectElements = container.querySelectorAll("select.episode-select");
    expect(selectElements.length).toBeGreaterThanOrEqual(1);

    const seasonSelect = selectElements[0];
    fireEvent.change(seasonSelect, { target: { value: "1" } });
    expect(seasonSelect.value).toBe("1");

    const updatedSelects = container.querySelectorAll("select.episode-select");
    expect(updatedSelects.length).toBeGreaterThanOrEqual(2);
    const episodeSelect = updatedSelects[1];
    fireEvent.change(episodeSelect, { target: { value: "1" } });
    expect(episodeSelect.value).toBe("1");

    await waitFor(() => {
      expect(screen.getByText(/Upload Video:/i)).toBeInTheDocument();
    });
  });

  test("calls clearUserData on updateFilm error in updateField", async () => {
    updateFilm.mockRejectedValueOnce({ response: { status: 401 } });
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "ADMIN",
      token: "dummy-token",
    };
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    renderWithRouter(<FilmDetail />);

    await waitFor(() => {
      expect(screen.getByText(/Фоллаут/)).toBeInTheDocument();
    });

    await act(async () => {
      await userEvent.click(screen.getAllByTestId("edit-image-button")[0]);
    })

    const titleInput = screen.getByDisplayValue(/Фоллаут/);
    fireEvent.change(titleInput, { target: { value: "New Title" } });
    fireEvent.blur(titleInput);

    await act(async () => {
      await userEvent.click(screen.getAllByTestId("save-image-button")[0]);
    })

    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("calls clearUserData on uploadImage error in handleThumbnailChange", async () => {
    uploadImage.mockRejectedValueOnce({ response: { status: 401 } });
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "ADMIN",
      token: "dummy-token",
    };
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    renderWithRouter(<FilmDetail />);

    const fileInput = await waitFor(() => screen.getByTestId("image-upload"));
    const file = new File(["dummy content"], "dummy.png", { type: "image/png" });
    fireEvent.change(fileInput, { target: { files: [file] } });

    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("calls clearUserData on uploadVideo error in handleVideoUpload", async () => {
    uploadVideo.mockRejectedValueOnce({ response: { status: 401 } });
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "ADMIN",
      token: "dummy-token",
    };
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    renderWithRouter(<FilmDetail />);

    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());
    const select1 = screen.getByRole("combobox");
    await act(() => {
      userEvent.selectOptions(select1, "1");
    })
    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument())
    const select2 = screen.getAllByRole("combobox")[1];
    await act(() => {
      userEvent.selectOptions(select2, "1");
    })

    const videoInput = await waitFor(() => screen.getByTestId("episode-upload"));
    const videoFile = new File(["dummy content"], "video.mp4", { type: "video/mp4" });
    fireEvent.change(videoInput, { target: { files: [videoFile] } });

    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("calls clearUserData on updateEpisode error in updateEpisodeField", async () => {
    updateEpisode.mockRejectedValueOnce({ response: { status: 401 } });
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "ADMIN",
      token: "dummy-token",
    };
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    renderWithRouter(<FilmDetail />);

    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());

    const select1 = screen.getByRole("combobox");
    await act(() => {
      userEvent.selectOptions(select1, "1");
    })
    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument())
    const select2 = screen.getAllByRole("combobox")[1];
    await act(() => {
      userEvent.selectOptions(select2, "1");
    })

    const episodeTitle = await waitFor(() => screen.getByText("Трейлер 1"));
    const container = episodeTitle.parentElement;
    const episodeTitleEditButton = within(container).getByTestId("edit-image-button");
    await act(() => {
      userEvent.click(episodeTitleEditButton);
    })
    const episodeTitleInput = await waitFor(() => screen.getByDisplayValue("Трейлер 1"));
    fireEvent.change(episodeTitleInput, { target: { value: "Updated Episode" } });
    fireEvent.blur(episodeTitleInput);
    const episodeTitleSaveButton = within(container).getByTestId("save-image-button");
    await act(() => {
      userEvent.click(episodeTitleSaveButton);
    })
    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("calls clearUserData on deleteEpisode error in deleteEpisodeById", async () => {
    deleteEpisode.mockRejectedValueOnce({ response: { status: 401 } });
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "ADMIN",
      token: "dummy-token",
    };
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());

    const select1 = screen.getByRole("combobox");
    await act(() => {
      userEvent.selectOptions(select1, "1");
    })
    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument())
    const select2 = screen.getAllByRole("combobox")[1];
    await act(() => {
      userEvent.selectOptions(select2, "1");
    })

    const deleteBtn = await waitFor(() => screen.getByRole("button", { name: /Delete/i }));
    userEvent.click(deleteBtn);

    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  const renderFilmAndSelectEpisode = async () => {
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());
    const seasonSelect = screen.getByRole("combobox");
    await act(async () => {
      userEvent.selectOptions(seasonSelect, "1");
    });
    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument());
    const episodeSelect = screen.getAllByRole("combobox")[1];
    await act(async () => {
      userEvent.selectOptions(episodeSelect, "1");
    });
  };

  const scenarios = [
    {
      userRole: "USER",
      mockStatus: "NOT_UPLOADED",
      expectedMessage: "Coming soon..."
    },
    {
      userRole: "ADMIN",
      mockStatus: "NOT_UPLOADED",
      expectedMessage: "The video has not been uploaded yet..."
    },
    {
      userRole: "ADMIN",
      mockStatus: "IN_PROGRESS",
      expectedMessage: "The video is uploading..."
    },
    {
      userRole: "USER",
      mockStatus: "IN_PROGRESS",
      expectedMessage: "The episode is uploading, check again in a short while!"
    },
    {
      userRole: "ADMIN",
      mockStatus: "ERROR",
      expectedMessage: "There was an error uploading the video!"
    },
    {
      userRole: "USER",
      mockStatus: "ERROR",
      expectedMessage: "Coming soon..."
    },
    {
      userRole: "ADMIN",
      mockStatus: "unexpected status",
      expectedMessage: "Coming soon..."
    }
  ];

  test.each(scenarios)( "displays correct message for $userRole and $mockStatus",
    async ({ userRole, mockStatus, expectedMessage }) => {
      getEpisodes.mockResolvedValueOnce([
        {
          id: "ep1",
          episode_num: 1,
          season_num: 1,
          title: "Test Episode",
          description: "",
          status: mockStatus,
          s3_bucket_name: null,
          content_id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a"
        }
      ]);
      AuthContext.useAuth().userData = {
        login: "login",
        email: "email@email.em",
        user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
        role: userRole,
        token: "dummy-token"
      };
      await renderFilmAndSelectEpisode();
      await waitFor(() => {
        expect(screen.getByText(expectedMessage)).toBeInTheDocument();
      });
    }
  );

  test("sets correct thumbnailSource when film has a thumbnail", async () => {
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "68b1ba18-b97c-4481-97d5-debaf9616182",
      role: "USER",
      token: "dummy-token"
    };
    getFilmById.mockResolvedValueOnce({
      id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
      title: "Фоллаут",
      genre: "DRAMA",
      category: "SERIES",
      publisher: "Amazon_Prime_Video",
      thumbnail: "/thumb.jpg",
      year: 2024,
      description: "A test film",
      cast_members: []
    });
  
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());
  
    const thumbImg = screen.getByAltText("Film thumbnail");
    expect(thumbImg).toHaveAttribute("src", `${apiBaseURL}/thumb.jpg`);
  });
  
  test("resets selectedSeason when no matching episode is found", async () => {
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());

    const seasonSelect = screen.getByRole("combobox");
    await act(async () => {
      userEvent.selectOptions(seasonSelect, "1");
    });
    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument());
    const episodeSelect = screen.getAllByRole("combobox")[1];
    fireEvent.change(episodeSelect, { target: { value: "2" } });

    await waitFor(() => {
      expect(screen.queryByRole("combobox", { name: /Select episode/i })).toBeNull();
    });
  });
  
  test("calls clearUserData when getPersonalList returns 401", async () => {
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;
    getPersonalList.mockRejectedValueOnce({ response: { status: 401 } });
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(mockClearUserData).toHaveBeenCalled());
  });
  
  test("calls clearUserData when addToPersonalList returns 401", async () => {
    const mockClearUserData = jest.fn();
    AuthContext.useAuth().clearUserData = mockClearUserData;

    getPersonalList.mockResolvedValueOnce([]);
    addToPersonalList.mockRejectedValueOnce({ response: { status: 401 } });

    renderWithRouter(<FilmDetail />);

    await waitFor(() => expect(screen.getByRole("button", { name: /Add to favorites/i })).toBeInTheDocument());
    userEvent.click(screen.getByRole("button", { name: /Add to favorites/i }));
    await waitFor(() => expect(mockClearUserData).toHaveBeenCalled());
  });
  
  test("calls clearUserData when removeFromPersonalList returns 401", async () => {
    const clearSpy = jest.fn();
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "user-123",
      role: "USER",
      token: "dummy-token"
    };
    AuthContext.useAuth().clearUserData = clearSpy;
    removeFromPersonalList.mockRejectedValueOnce({ response: { status: 401 } });
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByRole("button", { name: /Remove from favorites/i })).toBeInTheDocument());
    userEvent.click(screen.getByRole("button", { name: /Remove from favorites/i }));
    await waitFor(() => expect(clearSpy).toHaveBeenCalled());
  });
  
  test( "parses $field correctly when edited", async () => {
      getFilmById.mockResolvedValueOnce({
        id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
        title: "Фоллаут",
        genre: "ACTION_MOVIE",
        category: "SERIES",
        publisher: "Amazon_Prime_Video",
        thumbnail: null,
        year: 2024,
        description: "A test film",
        age_restriction: "SIXTEEN_PLUS",
        cast_members: []
      });
      AuthContext.useAuth().userData = {
        login: "login",
        email: "email@email.em",
        user_id: "user-123",
        role: "ADMIN",
        token: "dummy-token"
      };
      renderWithRouter(<FilmDetail />);
      await waitFor(() => expect(screen.getByText("Action movie")).toBeInTheDocument());
      await waitFor(() => expect(screen.getByText("16+")).toBeInTheDocument());
    }
  );
  
  test("resets selectedSeason when no matching (or previous) episode is found", async () => {
    getFilmById.mockResolvedValueOnce({
      id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
      title: "Фоллаут",
      genre: "DRAMA",
      category: "SERIES",
      publisher: "Amazon_Prime_Video",
      thumbnail: null,
      year: 2024,
      description: "A test film",
      cast_members: []
    });
    getEpisodes.mockResolvedValueOnce([
      {
        id: "ep2",
        episode_num: 2,
        season_num: 1,
        title: "Episode 2",
        description: "",
        status: "UPLOADED",
        s3_bucket_name: "/video2.mp4",
        content_id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a"
      }
    ]);
    AuthContext.useAuth().userData = {
      login: "login",
      email: "email@email.em",
      user_id: "user-123",
      role: "USER",
      token: "dummy-token"
    };
  
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());
    
    const seasonSelect = screen.getByRole("combobox");  
    await act(async () => {
      userEvent.selectOptions(seasonSelect, "1");
    });

    await waitFor(() => expect(screen.getByText("Select episode")).toBeInTheDocument());
    const episodeSelect = screen.getAllByRole("combobox")[1];

    await act(async () => {
      userEvent.selectOptions(episodeSelect, "2");
    });

    cleanup();
    getEpisodes.mockResolvedValueOnce([]);
    renderWithRouter(<FilmDetail />);
    await waitFor(() => expect(screen.getByText(/Фоллаут/)).toBeInTheDocument());
    
    await waitFor(() => {
      expect(screen.getAllByRole("combobox").length).toBeLessThan(2);
    });
  });
});
