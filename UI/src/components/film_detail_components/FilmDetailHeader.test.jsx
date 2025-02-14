import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import FilmDetailHeader from "./FilmDetailHeader";
import "@testing-library/jest-dom";

const filmInfo = { id: "film1", title: "Test Film" };
const thumbnailSource = "http://example.com/image.jpg";

const mockOnThumbnailChange = jest.fn();
const mockOnAddToPersonalList = jest.fn();
const mockOnRemoveFromPersonalList = jest.fn();

describe("FilmDetailHeader", () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  test("renders UpdatableImage when user is ADMIN", () => {
    const userData = { role: "ADMIN" };
    const personalList = [];

    render(
      <FilmDetailHeader
        filmInfo={filmInfo}
        thumbnailSource={thumbnailSource}
        userData={userData}
        personalList={personalList}
        onThumbnailChange={mockOnThumbnailChange}
        onAddToPersonalList={mockOnAddToPersonalList}
        onRemoveFromPersonalList={mockOnRemoveFromPersonalList}
      />
    );

    const uploadableImage = document.querySelector(".film-detail-logo-uploadable");
    expect(uploadableImage).toBeInTheDocument();
  });

  test("renders standard img when user is not ADMIN", () => {
    const userData = { role: "USER" };
    const personalList = [];

    render(
      <FilmDetailHeader
        filmInfo={filmInfo}
        thumbnailSource={thumbnailSource}
        userData={userData}
        personalList={personalList}
        onThumbnailChange={mockOnThumbnailChange}
        onAddToPersonalList={mockOnAddToPersonalList}
        onRemoveFromPersonalList={mockOnRemoveFromPersonalList}
      />
    );

    const image = screen.getByAltText("Film thumbnail");
    expect(image).toBeInTheDocument();
    expect(image).toHaveAttribute("src", thumbnailSource);
  });

  test("shows 'Add to favorites' button when film is not in personalList", () => {
    const userData = { role: "USER" };
    const personalList = [{ id: "film2" }];

    render(
      <FilmDetailHeader
        filmInfo={filmInfo}
        thumbnailSource={thumbnailSource}
        userData={userData}
        personalList={personalList}
        onThumbnailChange={mockOnThumbnailChange}
        onAddToPersonalList={mockOnAddToPersonalList}
        onRemoveFromPersonalList={mockOnRemoveFromPersonalList}
      />
    );

    const addButton = screen.getByRole("button", { name: /Add to favorites/i });
    expect(addButton).toBeInTheDocument();

    fireEvent.click(addButton);
    expect(mockOnAddToPersonalList).toHaveBeenCalledWith("film1");
  });

  test("shows 'Remove from favorites' button when film is in personalList", () => {
    const userData = { role: "USER" };
    const personalList = [{ id: "film1" }];

    render(
      <FilmDetailHeader
        filmInfo={filmInfo}
        thumbnailSource={thumbnailSource}
        userData={userData}
        personalList={personalList}
        onThumbnailChange={mockOnThumbnailChange}
        onAddToPersonalList={mockOnAddToPersonalList}
        onRemoveFromPersonalList={mockOnRemoveFromPersonalList}
      />
    );

    const removeButton = screen.getByRole("button", { name: /Remove from favorites/i });
    expect(removeButton).toBeInTheDocument();

    fireEvent.click(removeButton);
    expect(mockOnRemoveFromPersonalList).toHaveBeenCalledWith("film1");
  });
});
