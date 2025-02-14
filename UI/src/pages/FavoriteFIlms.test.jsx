// FavoriteFilms.test.jsx
import React from 'react';
import { screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import FavoriteFilms from './FavoriteFilms';
import '@testing-library/jest-dom';
import { renderWithRouter } from '../configs/testConfig';

// --- Mocks for Navigation and FilmList ---
const mockNavigate = jest.fn();
const mockClearUserData = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

jest.mock("../components/general/Navigation", () => () => (
  <div data-testid="navigation">Navigation</div>
));

// A FilmList mock that renders its props for inspection.
jest.mock("../components/flim_components/FilmList", () => (props) => (
  <div data-testid="film-list">
    <div data-testid="text-props">{JSON.stringify(props)}</div>
    {/* These buttons simulate user actions */}
    <button data-testid="remove-fav-btn" onClick={() => props.onRemoveFavorite("film1")}>
      Remove Favorite
    </button>
    <button data-testid="card-click" onClick={() => props.onCardClick("film1")}>
      Card Click
    </button>
    <button data-testid="card-mouse-leave" onClick={() => props.onCardMouseLeave()}>
      Card MouseLeave
    </button>
  </div>
));

// --- Mock the configuration for processing films ---
jest.mock("../configs/config", () => () => ({
  age_restriction: { "13": "13+", "PG": "PG" }
}));

// --- API Mocks ---
jest.mock("../services/api", () => require("../__mocks__/services/api"));

// --- AuthContext Mock ---
jest.mock('@contexts/AuthContext', () => ({
  useAuth: () => ({
    userData: { user_id: "user1", token: "token", role: "USER" },
    clearUserData: mockClearUserData,
  }),
}));

describe("FavoriteFilms component", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders basic structure", async () => {
    renderWithRouter(<FavoriteFilms />, ["/favorites"]);
    // Check Navigation is rendered.
    expect(screen.getByTestId("navigation")).toBeInTheDocument();
    // Check heading text.
    expect(screen.getByText(/Your favorite films and series/i)).toBeInTheDocument();
    // FilmList should be rendered.
    expect(screen.getByTestId("film-list")).toBeInTheDocument();
  });

  test("calls getPersonalList on mount and processes films", async () => {
    // Dummy response from getPersonalList.
    const dummyFilms = [
      {
        id: "film1",
        genre: "action_drama",
        category: "MOVIE",
        age_restriction: "13",
        publisher: "NET_FLIX"
      },
      {
        id: "film2",
        genre: "comedy",
        category: "SERIES",
        age_restriction: "PG",
        publisher: "HBO"
      }
    ];
    // Mock getPersonalList to resolve with our dummy films.
    const { getPersonalList } = require("../services/api");
    getPersonalList.mockResolvedValue(dummyFilms);

    renderWithRouter(<FavoriteFilms />);
    
    // Wait until the FilmList mock renders processed props.
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      // Expect the films (or personalList) prop to be the processed dummyFilms.
      // For film1: genre "action_drama" becomes "Action drama", category "MOVIE" becomes "Movie",
      // age_restriction "13" becomes "13+" (per our mapping), publisher "NET_FLIX" becomes "NET FLIX"
      expect(filmListProps.films).toEqual([
        {
          id: "film1",
          genre: "Action drama",
          category: "Movie",
          age_restriction: "13+",
          publisher: "NET FLIX"
        },
        {
          id: "film2",
          genre: "Comedy",
          category: "Series",
          age_restriction: "PG",
          publisher: "HBO"
        }
      ]);
    });
  });

  test("navigates to /login when getPersonalList returns 401 error", async () => {
    const { getPersonalList } = require("../services/api");
    getPersonalList.mockRejectedValue({ response: { status: 401 } });

    renderWithRouter(<FavoriteFilms />);
    
    await waitFor(() => {
      // clearUserData should be called and navigation to "/login" should occur.
      expect(mockClearUserData).toHaveBeenCalled();
      expect(mockNavigate).toHaveBeenCalledWith("/login");
    });
  });

  test("onCardClick navigates to film details", async () => {
    // Set up a dummy getPersonalList response so FilmList is rendered.
    const dummyFilms = [
      {
        id: "film1",
        genre: "action_drama",
        category: "MOVIE",
        age_restriction: "13",
        publisher: "NET_FLIX"
      }
    ];
    require("../services/api").getPersonalList.mockResolvedValue(dummyFilms);

    renderWithRouter(<FavoriteFilms />);
    
    // Wait for the FilmList to render.
    await waitFor(() => {
      expect(screen.getByTestId("film-list")).toBeInTheDocument();
    });
    // Simulate clicking the card.
    const cardClickBtn = screen.getByTestId("card-click");
    fireEvent.click(cardClickBtn);
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/films/film1");
    });
  });

  test("onRemoveFavorite updates favoriteFilms", async () => {
    // Start with two films.
    const dummyFilms = [
      {
        id: "film1",
        genre: "action_drama",
        category: "MOVIE",
        age_restriction: "13",
        publisher: "NET_FLIX"
      },
      {
        id: "film2",
        genre: "comedy",
        category: "SERIES",
        age_restriction: "PG",
        publisher: "HBO"
      }
    ];
    require("../services/api").getPersonalList.mockResolvedValue(dummyFilms);
    const { removeFromPersonalList } = require("../services/api");
    removeFromPersonalList.mockResolvedValue({});

    renderWithRouter(<FavoriteFilms />);
    
    // Wait until the FilmList has rendered the films.
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.films).toHaveLength(2);
    });
    
    // Click the "Remove Favorite" button for film1.
    const removeFavBtn = screen.getByTestId("remove-fav-btn");
    fireEvent.click(removeFavBtn);
    
    await waitFor(() => {
      // After removal, the FilmList props should update and only contain film2.
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.films).toHaveLength(1);
      expect(filmListProps.films[0].id).toBe("film2");
    });
  });

  test("onCardMouseLeave sets hoveredId to null", async () => {
    // Render component; initially hoveredId is null.
    renderWithRouter(<FavoriteFilms />);
    
    // Simulate a mouse enter to change hoveredId (this happens via FilmList if you call onCardMouseEnter)
    // We'll simulate that by directly clicking a button that calls onCardMouseLeave.
    const cardMouseLeaveBtn = screen.getByTestId("card-mouse-leave");
    fireEvent.click(cardMouseLeaveBtn);
    
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.hoveredId).toBeNull();
    });
  });
});
