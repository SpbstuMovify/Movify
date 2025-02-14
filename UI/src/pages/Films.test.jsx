import React from 'react';
import { screen, fireEvent, waitFor, act } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Films from './Films';
import { useFilms } from '../hooks/useFilms';
import '@testing-library/jest-dom';
import { renderWithRouter } from '../configs/testConfig';

const mockClearUserData = jest.fn();

const mockNavigate = jest.fn();
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
    <button data-testid="delete-btn" onClick={() => props.onDeleteFilm("film1")}>
      Delete
    </button>
    <button data-testid="add-fav-btn" onClick={() => props.onAddFavorite("film1")}>
      Add Favorite
    </button>
    <button data-testid="remove-fav-btn" onClick={() => props.onRemoveFavorite("film1")}>
      Remove Favorite
    </button>
    <button data-testid="card-mouse-leave" onClick={() => props.onCardMouseLeave()}>
      Card MouseLeave
    </button>
    <button data-testid="card-click" onClick={() => props.onCardClick("film1")}>
      Card Click
    </button>
  </div>
));

  
jest.mock("../components/flim_components/SearchForm", () => (props) => (
  <form data-testid="search-form" onSubmit={props.onSubmit}>
    <input placeholder="Search..." {...props.register("title")} />
    <button type="submit">Search</button>
    {props.errors.root && <span>{props.errors.root}</span>}
  </form>
));
jest.mock("../components/flim_components/FilterPanel", () => () => (
  <div data-testid="filter-panel">FilterPanel</div>
));
jest.mock("../components/flim_components/Pagination", () => (props) => (
  <div data-testid="pagination">
    <button data-testid="page-back" onClick={props.onPageBack}>Back</button>
    <span data-testid="page-number">{props.pageNumber}</span>
    <button data-testid="page-forward" onClick={props.onPageForward}>Forward</button>
    <button data-testid="jump-first" onClick={props.onJumpFirst}>Jump First</button>
    <button data-testid="jump-last" onClick={props.onJumpLast}>Jump Last</button>
    <select data-testid="page-size" onChange={props.onPageSizeChange} value={props.pageSize}>
      {props.pageSizeOptions.map((option) => (
        <option key={option} value={option}>{option}</option>
      ))}
    </select>
  </div>
));

jest.mock('../services/api', () => require("../__mocks__/services/api"));
jest.mock('@contexts/AuthContext', () => require('../__mocks__/contexts/AuthContext'));

const mockRefetchFilms = jest.fn();
const mockSetPageSize = jest.fn();
const mockSetPageNumber = jest.fn();
jest.mock("../hooks/useFilms", () => ({
  useFilms: jest.fn(() => ({
    films: [{ id: "film1", title: "Film One" }],
    pageSize: 10,
    setPageSize: mockSetPageSize,
    pageNumber: 0,
    setPageNumber: mockSetPageNumber,
    maxPageNumber: 2,
    refetchFilms: mockRefetchFilms,
  })),
}));

describe("Films component", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("renders basic structure", () => {
    renderWithRouter(<Films />, ["/films"]);
    expect(screen.getByTestId("navigation")).toBeInTheDocument();
    expect(screen.getByTestId("search-form")).toBeInTheDocument();
    expect(screen.getByTestId("film-list")).toBeInTheDocument();
    expect(screen.getByTestId("pagination")).toBeInTheDocument();
    expect(screen.getByTestId("filter-panel")).toBeInTheDocument();
  });

  test("updates queryParams and navigates on search submission", async () => {
    renderWithRouter(<Films />, ["/films?title=old"]);
    const searchInput = screen.getByPlaceholderText("Search...");

    await userEvent.clear(searchInput);
    await userEvent.type(searchInput, "newsearch");
    
    fireEvent.submit(screen.getByTestId("search-form"));
    
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/films?title=newsearch");
    });
  });

  test("calls getPersonalList and passes personalList to FilmList", async () => {
    const dummyPersonalList = [{ id: "film1" }, { id: "film2" }];
    require("../services/api").getPersonalList.mockResolvedValue(dummyPersonalList);

    renderWithRouter(<Films />);

    await waitFor(() => {
      const filmListProps = JSON.parse(screen.getByTestId("text-props").textContent);
      expect(filmListProps.personalList).toEqual(dummyPersonalList);
    });
  });

  test("handleAddNewFilm calls createFilm and refetchFilms", async () => {
    require("../services/api").createFilm.mockResolvedValue({});
    jest.spyOn(require("../contexts/AuthContext"), "useAuth").mockReturnValue({
      userData: { user_id: "user1", token: "token", role: "ADMIN" },
      clearUserData: jest.fn(),
    });
    renderWithRouter(<Films />);
    const addNewFilmButton = screen.getByRole("button", { name: /Add new film/i });
    fireEvent.click(addNewFilmButton);
    await waitFor(() => {
      expect(require("../services/api").createFilm).toHaveBeenCalledWith("token");
      expect(mockRefetchFilms).toHaveBeenCalled();
    });
  });

  test("handleDeleteFilm calls deleteFilm and refetchFilms", async () => {
    require("../services/api").deleteFilm.mockResolvedValue({});
    renderWithRouter(<Films />);
    
    const deleteButton = await screen.findByTestId("delete-btn");
    fireEvent.click(deleteButton);
    
    await waitFor(() => {
      expect(require("../services/api").deleteFilm).toHaveBeenCalledWith("token", "film1");
      expect(mockRefetchFilms).toHaveBeenCalled();
    });
  });
  
  test("handleAddToPersonalList calls addToPersonalList", async () => {
    const dummyResponse = { content_id: "film1" };
    require("../services/api").addToPersonalList.mockResolvedValue(dummyResponse);
    renderWithRouter(<Films />);
    
    const addFavButton = await screen.findByTestId("add-fav-btn");
    fireEvent.click(addFavButton);
    
    await waitFor(() => {
      expect(require("../services/api").addToPersonalList).toHaveBeenCalledWith("user1", "film1", "token");
    });
  });
  
  test("handleRemoveFromPersonalList calls removeFromPersonalList", async () => {
    require("../services/api").removeFromPersonalList.mockResolvedValue({});
    renderWithRouter(<Films />);
    
    const removeFavButton = await screen.findByTestId("remove-fav-btn");
    fireEvent.click(removeFavButton);
    
    await waitFor(() => {
      expect(require("../services/api").removeFromPersonalList).toHaveBeenCalledWith("user1", "film1", "token");
    });
  });

  test("handleAddToPersonalList error (401) calls clearUserData", async () => {
    require("../services/api").addToPersonalList.mockRejectedValue({ response: { status: 401 } });
    jest.spyOn(require("../contexts/AuthContext"), "useAuth").mockReturnValue({
      userData: { user_id: "user1", token: "token", role: "ADMIN" },
      clearUserData: mockClearUserData,
    });
    renderWithRouter(<Films />);
    const addFavButton = await screen.findByTestId("add-fav-btn");
    fireEvent.click(addFavButton);
    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("handleRemoveFromPersonalList error (401) calls clearUserData", async () => {
    require("../services/api").removeFromPersonalList.mockRejectedValue({ response: { status: 401 } });
    jest.spyOn(require("../contexts/AuthContext"), "useAuth").mockReturnValue({
      userData: { user_id: "user1", token: "token", role: "ADMIN" },
      clearUserData: mockClearUserData,
    });
    renderWithRouter(<Films />);
    const removeFavButton = await screen.findByTestId("remove-fav-btn");
    fireEvent.click(removeFavButton);
    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });


  test("handleAddNewFilm error (401) calls clearUserData", async () => {
    require("../services/api").createFilm.mockRejectedValue({ response: { status: 401 } });
    jest.spyOn(require("../contexts/AuthContext"), "useAuth").mockReturnValue({
      userData: { user_id: "user1", token: "token", role: "ADMIN" },
      clearUserData: mockClearUserData,
    });
    renderWithRouter(<Films />);
    const addNewFilmButton = screen.getByRole("button", { name: /Add new film/i });
    fireEvent.click(addNewFilmButton);
    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("handleDeleteFilm error (401) calls clearUserData", async () => {
    require("../services/api").deleteFilm.mockRejectedValue({ response: { status: 401 } });
    renderWithRouter(<Films />);
    const deleteButton = await screen.findByTestId("delete-btn");
    fireEvent.click(deleteButton);
    await waitFor(() => {
      expect(mockClearUserData).toHaveBeenCalled();
    });
  });

  test("onCardMouseLeave sets hoveredId to null", async () => {
    renderWithRouter(<Films />);
    const cardMouseLeaveBtn = await screen.findByTestId("card-mouse-leave");
    fireEvent.click(cardMouseLeaveBtn);
    const filmListProps = JSON.parse(screen.getByTestId("text-props").textContent);
    expect(filmListProps.hoveredId).toBeNull();
  });

  test("onCardClick navigates to film details", async () => {
    renderWithRouter(<Films />);
    const cardClickBtn = await screen.findByTestId("card-click");
    fireEvent.click(cardClickBtn);
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/films/film1");
    });
  });

  test("pagination onPageBack calls setPageNumber when pageNumber > 0", async () => {
    useFilms.mockReturnValue({
      films: [],
      pageSize: 10,
      setPageSize: mockSetPageSize,
      pageNumber: 1,
      setPageNumber: mockSetPageNumber,
      maxPageNumber: 2,
      refetchFilms: mockRefetchFilms,
    });
    renderWithRouter(<Films />);
    const pageBackBtn = screen.getByTestId("page-back");
    fireEvent.click(pageBackBtn);
    expect(mockSetPageNumber).toHaveBeenCalledWith(0);
  });


  test("pagination onPageForward calls setPageNumber when pageNumber < maxPageNumber", async () => {
    useFilms.mockReturnValue({
      films: [{ id: "film1", title: "Film One" }],
      pageSize: 10,
      setPageSize: mockSetPageSize,
      pageNumber: 0,
      setPageNumber: mockSetPageNumber,
      maxPageNumber: 2,
      refetchFilms: mockRefetchFilms,
    });
    renderWithRouter(<Films />);
    const pageForwardBtn = screen.getByTestId("page-forward");
    await userEvent.click(pageForwardBtn);
    expect(mockSetPageNumber).toHaveBeenCalledWith(1);
  });

  test("pagination onJumpFirst calls setPageNumber(0)", async () => {
    renderWithRouter(<Films />);
    const jumpFirstBtn = screen.getByTestId("jump-first");
    fireEvent.click(jumpFirstBtn);
    expect(mockSetPageNumber).toHaveBeenCalledWith(0);
  });

  test("pagination onJumpLast calls setPageNumber(maxPageNumber)", async () => {
    renderWithRouter(<Films />);
    const jumpLastBtn = screen.getByTestId("jump-last");
    fireEvent.click(jumpLastBtn);
    expect(mockSetPageNumber).toHaveBeenCalledWith(2);
  });

  test("pagination onPageSizeChange calls setPageSize with selected value", async () => {
    renderWithRouter(<Films />);
    const pageSizeSelect = screen.getByTestId("page-size");
    fireEvent.change(pageSizeSelect, { target: { value: "20" } });
    expect(mockSetPageSize).toHaveBeenCalledWith("20");
  });
});
