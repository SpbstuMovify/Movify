import React from 'react';
import { screen, fireEvent, waitFor } from '@testing-library/react';
import FavoriteFilms from './FavoriteFilms';
import '@testing-library/jest-dom';
import { renderWithRouter } from '../configs/testConfig';

const mockNavigate = jest.fn();
const mockClearUserData = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useNavigate: () => mockNavigate,
}));

jest.mock("../components/general/Navigation", () => () => (
  <div data-testid="navigation">Navigation</div>
));

jest.mock("../components/flim_components/FilmList", () => (props) => (
  <div data-testid="film-list">
    <div data-testid="text-props">{JSON.stringify(props)}</div>
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

jest.mock("../configs/config", () => () => ({
  age_restriction: { "13": "13+", "PG": "PG" }
}));

jest.mock("../services/api", () => require("../__mocks__/services/api"));

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
    expect(screen.getByTestId("navigation")).toBeInTheDocument();
    expect(screen.getByText(/Your favorite films and series/i)).toBeInTheDocument();
    expect(screen.getByTestId("film-list")).toBeInTheDocument();
  });

  test("calls getPersonalList on mount and processes films", async () => {
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
    const { getPersonalList } = require("../services/api");
    getPersonalList.mockResolvedValue(dummyFilms);

    renderWithRouter(<FavoriteFilms />);
    
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
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
      expect(mockClearUserData).toHaveBeenCalled();
      expect(mockNavigate).toHaveBeenCalledWith("/login");
    });
  });

  test("onCardClick navigates to film details", async () => {
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
    
    await waitFor(() => {
      expect(screen.getByTestId("film-list")).toBeInTheDocument();
    });
    const cardClickBtn = screen.getByTestId("card-click");
    fireEvent.click(cardClickBtn);
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/films/film1");
    });
  });

  test("onRemoveFavorite updates favoriteFilms", async () => {
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
    
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.films).toHaveLength(2);
    });
    
    const removeFavBtn = screen.getByTestId("remove-fav-btn");
    fireEvent.click(removeFavBtn);
    
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.films).toHaveLength(1);
      expect(filmListProps.films[0].id).toBe("film2");
    });
  });

  test("onCardMouseLeave sets hoveredId to null", async () => {
    renderWithRouter(<FavoriteFilms />);
    
    const cardMouseLeaveBtn = screen.getByTestId("card-mouse-leave");
    fireEvent.click(cardMouseLeaveBtn);
    
    await waitFor(() => {
      const propsJSON = screen.getByTestId("text-props").textContent;
      const filmListProps = JSON.parse(propsJSON);
      expect(filmListProps.hoveredId).toBeNull();
    });
  });
});
