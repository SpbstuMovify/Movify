import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useForm } from "react-hook-form";
import FilmEpisodesPanel from "./FilmEpisodesPanel";
import "@testing-library/jest-dom";

jest.mock("react-player", () => (props) => (
  <div data-testid="react-player">{props.url}</div>
));

jest.mock("../general/EditableField", () => (props) => {
  const testId = props.label ? `save-${props.label}` : "save-title";
  return (
    <div data-testid="editable-field">
      <span data-testid="field-value">{props.value}</span>
      { !props.disableEditButton && (
        <button data-testid={testId} onClick={() => props.onSave("new value")}>
          Save
        </button>
      )}
    </div>
  );
});

const TestFilmEpisodesPanel = ({
  seasonArray = [1, 2],
  selectedSeason = "",
  setSelectedSeason = jest.fn(),
  seasonEpisodeArray = [],
  selectedEpisode = null,
  setSelectedEpisode = jest.fn(),
  filmEpisodesFormOverrides = {},
  userData = { role: "ADMIN" },
  processEpisodeStatus = (status) => `Status: ${status}`,
  handleVideoUpload = jest.fn(),
  updateEpisodeField = jest.fn(),
  deleteEpisodeById = jest.fn(),
}) => {
  const filmEpisodesForm = useForm({
    defaultValues: { ...filmEpisodesFormOverrides },
  });
  return (
    <FilmEpisodesPanel
      seasonArray={seasonArray}
      selectedSeason={selectedSeason}
      setSelectedSeason={setSelectedSeason}
      seasonEpisodeArray={seasonEpisodeArray}
      selectedEpisode={selectedEpisode}
      setSelectedEpisode={setSelectedEpisode}
      filmEpisodesForm={filmEpisodesForm}
      processEpisodeStatus={processEpisodeStatus}
      handleVideoUpload={handleVideoUpload}
      updateEpisodeField={updateEpisodeField}
      deleteEpisodeById={deleteEpisodeById}
      userData={userData}
    />
  );
};

describe("FilmEpisodesPanel", () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  test("renders season select with correct options", () => {
    const seasonArray = [1, 2, 3];
    render(
      <TestFilmEpisodesPanel
        seasonArray={seasonArray}
        selectedSeason={""}
        seasonEpisodeArray={[]}
        selectedEpisode={null}
      />
    );
    const seasonSelect = screen.getByRole("combobox");
    expect(seasonSelect).toHaveDisplayValue("Select season");
    seasonArray.forEach((season) => {
      expect(screen.getByRole("option", { name: `Season ${season}` })).toBeInTheDocument();
    });
  });

  test("changes selectedSeason and resets selectedEpisode on season select change", () => {
    const setSelectedSeason = jest.fn();
    const setSelectedEpisode = jest.fn();
    render(
      <TestFilmEpisodesPanel
        selectedSeason={""}
        setSelectedSeason={setSelectedSeason}
        seasonEpisodeArray={[]}
        selectedEpisode={null}
        setSelectedEpisode={setSelectedEpisode}
      />
    );
    const seasonSelect = screen.getByRole("combobox");
    fireEvent.change(seasonSelect, { target: { value: 2 } });
    expect(setSelectedSeason).toHaveBeenCalledWith(Number(2));
    expect(setSelectedEpisode).toHaveBeenCalledWith(null);
  });

  test("renders episode select when selectedSeason is provided and calls setSelectedEpisode on change", () => {
    const setSelectedEpisode = jest.fn();
    const seasonEpisodeArray = [
      { id: "ep1", episode_num: 1, title: "Episode 1", description: "Desc 1", status: "UPLOADED", s3_bucket_name: "/video1.mp4" },
      { id: "ep2", episode_num: 2, title: "Episode 2", description: "Desc 2", status: "PROCESSING", s3_bucket_name: "/video2.mp4" },
    ];
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={seasonEpisodeArray}
        selectedEpisode={null}
        setSelectedEpisode={setSelectedEpisode}
      />
    );
    const episodeSelect = screen.getAllByRole("combobox", { name: "" })[1];
    fireEvent.change(episodeSelect, { target: { value: "2" } });
    expect(setSelectedEpisode).toHaveBeenCalledWith(seasonEpisodeArray[1]);
  });

  test("renders ReactPlayer when selectedEpisode status is UPLOADED", () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
      />
    );
    const player = screen.getByTestId("react-player");
    expect(player).toBeInTheDocument();
    expect(player).toHaveTextContent("http://localhost:8090/video1.mp4");
  });

  test("renders placeholder when selectedEpisode status is not UPLOADED", () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "PROCESSING",
      s3_bucket_name: "/video1.mp4",
    };
    const processEpisodeStatus = jest.fn(() => "Processing...");
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        processEpisodeStatus={processEpisodeStatus}
      />
    );
    const placeholder = screen.getByText("Processing...");
    expect(placeholder).toBeInTheDocument();
    expect(placeholder).toHaveClass("player-placeholder");
  });

  test("calls updateEpisodeField when saving title via EditableField", async () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    const updateEpisodeField = jest.fn();
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        updateEpisodeField={updateEpisodeField}
      />
    );
    const saveTitleButton = screen.getByTestId("save-title");
    fireEvent.click(saveTitleButton);
    expect(updateEpisodeField).toHaveBeenCalledWith(selectedEpisode.id, "title", "new value");
  });

  test("calls updateEpisodeField when saving description via EditableField", async () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    const updateEpisodeField = jest.fn();
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        updateEpisodeField={updateEpisodeField}
      />
    );
    const saveDescButton = screen.getByTestId("save-Description:");
    fireEvent.click(saveDescButton);
    expect(updateEpisodeField).toHaveBeenCalledWith(selectedEpisode.id, "description", "new value");
  });

  test("calls handleVideoUpload when a video file is selected", async () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    const handleVideoUpload = jest.fn();
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        handleVideoUpload={handleVideoUpload}
        userData={{ role: "ADMIN" }}
      />
    );
    const fileInput = screen.getByTestId("episode-upload");
    const file = new File(["dummy content"], "video.mp4", { type: "video/mp4" });
    fireEvent.change(fileInput, { target: { files: [file] } });
    expect(handleVideoUpload).toHaveBeenCalledWith(selectedEpisode.id, file);
  });

  test("calls deleteEpisodeById when delete button is clicked", async () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    const deleteEpisodeById = jest.fn();
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        deleteEpisodeById={deleteEpisodeById}
        userData={{ role: "ADMIN" }}
      />
    );
    const deleteButton = screen.getByRole("button", { name: /Delete/i });
    fireEvent.click(deleteButton);
    expect(deleteEpisodeById).toHaveBeenCalledWith(selectedEpisode.id);
  });

  test("disables editing for episode fields when user is not ADMIN", () => {
    const selectedEpisode = {
      id: "ep1",
      episode_num: 1,
      title: "Episode 1",
      description: "Desc 1",
      status: "UPLOADED",
      s3_bucket_name: "/video1.mp4",
    };
    render(
      <TestFilmEpisodesPanel
        selectedSeason={"1"}
        seasonEpisodeArray={[selectedEpisode]}
        selectedEpisode={selectedEpisode}
        userData={{ role: "USER" }}
      />
    );
    const saveTitleButton = screen.queryByTestId("save-title");
    const saveDescButton = screen.queryByTestId("save-Description:");
    expect(saveTitleButton).toBeNull();
    expect(saveDescButton).toBeNull();
  });
});
