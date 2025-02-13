import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import FilmCard from './FilmCard';
import { apiBaseURL } from '../../services/api';
import '@testing-library/jest-dom';

describe('FilmCard component', () => {
  const film = {
    id: '1',
    title: 'Test Film',
    thumbnail: '/test_thumbnail.jpg',
    age_restriction: '16+',
    description: 'A test film description',
    publisher: 'Test Publisher',
    year: 2021,
    category: 'Test Category',
    genre: 'Test Genre',
  };

  const defaultProps = {
    film,
    isHovered: false,
    onMouseEnter: jest.fn(),
    onMouseLeave: jest.fn(),
    onClick: jest.fn(),
    onAddFavorite: jest.fn(),
    onRemoveFavorite: jest.fn(),
    onDeleteFilm: jest.fn(), // <-- Added here
    userData: { id: 'user1', role: 'USER' },
    personalList: [],
  };

  afterEach(() => {
    jest.clearAllMocks();
  });

  test('renders FilmCard with no thumbnail (shows default image)', () => {
    const filmNoThumbnail = { ...film, thumbnail: null };
    render(<FilmCard {...defaultProps} film={filmNoThumbnail} />);
    const img = screen.getByTestId('film-logo');
    expect(img).toHaveAttribute('src', '/images/no_image.jpg');
  });

  test('renders FilmCard with thumbnail', () => {
    render(<FilmCard {...defaultProps} />);
    const img = screen.getByTestId('film-logo');
    expect(img).toHaveAttribute('src', `${apiBaseURL}${film.thumbnail}`);
  });

  test('applies hover class when isHovered is true', () => {
    const { container } = render(<FilmCard {...defaultProps} isHovered={true} />);
    expect(container.firstChild).toHaveClass('film-element film-element-hover');
  });

  test('applies non-hover class when isHovered is false', () => {
    const { container } = render(<FilmCard {...defaultProps} isHovered={false} />);
    expect(container.firstChild).toHaveClass('film-element');
    expect(container.firstChild).not.toHaveClass('film-element-hover');
  });

  test('calls onMouseEnter and onMouseLeave when hovering over the card', () => {
    render(<FilmCard {...defaultProps} />);
    const outerDiv = screen.getByText(film.title).closest('div');
    fireEvent.mouseEnter(outerDiv);
    expect(defaultProps.onMouseEnter).toHaveBeenCalledWith(film.id);
    fireEvent.mouseLeave(outerDiv);
    expect(defaultProps.onMouseLeave).toHaveBeenCalled();
  });

  test('calls onClick when the card is clicked', () => {
    render(<FilmCard {...defaultProps} />);
    const outerDiv = screen.getByText(film.title).closest('div');
    fireEvent.click(outerDiv);
    expect(defaultProps.onClick).toHaveBeenCalledWith(film.id);
  });

  test('renders "Add to favorites" button when film is not favorite and calls onAddFavorite', () => {
    render(<FilmCard {...defaultProps} userData={{ id: 'user1' }} personalList={[]} />);
    const addFavImg = screen.getByAltText('Add to favorites');
    expect(addFavImg).toBeInTheDocument();
    fireEvent.click(addFavImg);
    expect(defaultProps.onAddFavorite).toHaveBeenCalledWith(film.id);
  });

  test('renders "Remove from favorites" button when film is favorite and calls onRemoveFavorite', () => {
    render(<FilmCard {...defaultProps} userData={{ id: 'user1' }} personalList={[film]} />);
    const removeFavImg = screen.getByAltText('Remove from favorites');
    expect(removeFavImg).toBeInTheDocument();
    fireEvent.click(removeFavImg);
    expect(defaultProps.onRemoveFavorite).toHaveBeenCalledWith(film.id);
  });

  test('does not render favorite buttons when userData is not provided', () => {
    render(<FilmCard {...defaultProps} userData={null} />);
    expect(screen.queryByAltText('Add to favorites')).toBeNull();
    expect(screen.queryByAltText('Remove from favorites')).toBeNull();
  });

  test('renders delete button for admin and calls onDeleteFilm when clicked', async () => {
    const adminUser = { id: 'admin1', role: 'ADMIN' };
    const props = {
      ...defaultProps,
      userData: adminUser,
    };
  
    render(<FilmCard {...props} />);
    const deleteButton = screen.getByText('Delete');
    expect(deleteButton).toBeInTheDocument();
    
    fireEvent.click(deleteButton);
    
    await waitFor(() => {
      expect(props.onDeleteFilm).toHaveBeenCalledWith(film.id);
    });
  });

  test('renders publisher and release year correctly', () => {
    render(<FilmCard {...defaultProps} />);
    const infoText = screen.getByText(new RegExp(`Publisher: ${film.publisher}, Release year: ${film.year}`));
    expect(infoText).toBeInTheDocument();
  });

  test('calls onMouseLeave and onMouseEnter for add favorites button mouse events', () => {
    // Set up fresh mocks for these events.
    const onMouseEnterMock = jest.fn();
    const onMouseLeaveMock = jest.fn();
    const props = {
      ...defaultProps,
      onMouseEnter: onMouseEnterMock,
      onMouseLeave: onMouseLeaveMock,
      userData: { id: 'user1' },
      personalList: []  // film is NOT a favorite, so "Add to favorites" is rendered.
    };
  
    render(<FilmCard {...props} />);
    const addFavButton = screen.getByAltText('Add to favorites').closest('button');
    fireEvent.mouseEnter(addFavButton);
    expect(onMouseLeaveMock).toHaveBeenCalled();
    fireEvent.mouseLeave(addFavButton);
    expect(onMouseEnterMock).toHaveBeenCalledWith(film.id);
  });
  
  test('calls onMouseLeave and onMouseEnter for remove favorites button mouse events', () => {
    const onMouseEnterMock = jest.fn();
    const onMouseLeaveMock = jest.fn();
    const props = {
      ...defaultProps,
      onMouseEnter: onMouseEnterMock,
      onMouseLeave: onMouseLeaveMock,
      userData: { id: 'user1' },
      personalList: [film]
    };
  
    render(<FilmCard {...props} />);
    const removeFavButton = screen.getByAltText('Remove from favorites').closest('button');
    fireEvent.mouseEnter(removeFavButton);
    expect(onMouseLeaveMock).toHaveBeenCalled();
    fireEvent.mouseLeave(removeFavButton);
    expect(onMouseEnterMock).toHaveBeenCalledWith(film.id);
  });
});
