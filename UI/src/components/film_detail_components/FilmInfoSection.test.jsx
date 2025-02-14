import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { useForm } from "react-hook-form";
import FilmInfoSection from "./FilmInfoSection";
import "@testing-library/jest-dom";

jest.mock("../general/EditableField", () => (props) => (
  <div data-testid="editable-field">
    <span data-testid="field-value">{props.value}</span>
    { !props.disableEditButton && (
      <button data-testid="save-button" onClick={() => props.onSave("new value")}>
        Save
      </button>
    )}
  </div>
));

const defaultFormValues = {
  title: "Test Film",
  genre: "action",         
  category: "movie",       
  age_restriction: "13",   
  publisher: "Test Publisher",
  year: 2000,
  description: "Test description",
  cast_members: [
    { employee_full_name: "Actor One", role_name: "Role One" }
  ],
};

const defaultFilmInfo = {
  id: "film1",
  cast_members: [
    { employee_full_name: "Actor One", role_name: "Role One" }
  ],
};

const TestComponent = ({
  filmInfo = defaultFilmInfo,
  userData = { role: "ADMIN" },
  updateField = jest.fn(),
  filmFormOverrides = {},
}) => {
  const filmForm = useForm({
    defaultValues: { ...defaultFormValues, ...filmFormOverrides },
  });
  return (
    <FilmInfoSection
      filmInfo={filmInfo}
      filmForm={filmForm}
      userData={userData}
      updateField={updateField}
      mappingJSON={() => ({
        genre_options: ["ACTION", "DRAMA"],
        category_options: ["MOVIE", "SERIES"],
        age_restriction_options: ["13", "PG"],
        age_restriction: { "13": "13+", "PG": "PG" },
      })}
      currentYear={2023}
    />
  );
};

const YearErrorTestComponent = ({ updateField = jest.fn() }) => {
  const filmForm = useForm({ defaultValues: { ...defaultFormValues } });
  React.useEffect(() => {
    filmForm.setError("year", {
      type: "validate",
      message: "Please enter a valid number",
    });
  }, [filmForm]);
  return (
    <FilmInfoSection
      filmInfo={defaultFilmInfo}
      filmForm={filmForm}
      userData={{ role: "ADMIN" }}
      updateField={updateField}
      mappingJSON={() => ({
        genre_options: ["ACTION", "DRAMA"],
        category_options: ["MOVIE", "SERIES"],
        age_restriction_options: ["13", "PG"],
        age_restriction: { "13": "13+", "PG": "PG" },
      })}
      currentYear={2023}
    />
  );
};

describe("FilmInfoSection", () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  test("renders null when filmInfo is not provided", () => {
    const { container } = render(
      <TestComponent filmInfo={null} userData={{ role: "ADMIN" }} updateField={jest.fn()} />
    );
    expect(container.firstChild).toBeNull();
  });

  test("renders all editable fields with correct values", async () => {
    render(<TestComponent />);
    const fields = await screen.findAllByTestId("editable-field");
    expect(fields).toHaveLength(9);
    
    expect(screen.getAllByTestId("field-value")[0]).toHaveTextContent("Test Film");        
    expect(screen.getAllByTestId("field-value")[1]).toHaveTextContent("ACTION");         
    expect(screen.getAllByTestId("field-value")[2]).toHaveTextContent("MOVIE");             
    expect(screen.getAllByTestId("field-value")[3]).toHaveTextContent("13");              
    expect(screen.getAllByTestId("field-value")[4]).toHaveTextContent("Test Publisher");    
    expect(screen.getAllByTestId("field-value")[5]).toHaveTextContent("2000");              
    expect(screen.getAllByTestId("field-value")[6]).toHaveTextContent("Test description");  
    expect(screen.getAllByTestId("field-value")[7]).toHaveTextContent("Actor One");          
    expect(screen.getAllByTestId("field-value")[8]).toHaveTextContent("Role One");           
  });

  test("calls updateField when saving a field (title)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[0]);
    expect(updateFieldMock).toHaveBeenCalledWith("title", "new value");
  });

  test("calls updateField when saving a field (genre)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[1]);
    expect(updateFieldMock).toHaveBeenCalledWith("genre", "new value", undefined);
  });

  test("calls updateField when saving a field (category)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[2]);
    expect(updateFieldMock).toHaveBeenCalledWith("category", "new value", undefined);
  });

  test("calls updateField when saving a field (age_restriction)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[3]);
    expect(updateFieldMock).toHaveBeenCalledWith("age_restriction", "new value", undefined);
  });

  test("calls updateField when saving a field (publisher)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[4]);
    expect(updateFieldMock).toHaveBeenCalledWith("publisher", "new value");
  });

  test("calls updateField when saving a field (year)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[5]);
    expect(updateFieldMock).toHaveBeenCalledWith("year", "new value", undefined);
  });

  test("calls updateField when saving a field (description)", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[6]);
    expect(updateFieldMock).toHaveBeenCalledWith("description", "new value");
  });

  test("calls updateField when saving a cast member's employee_full_name", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[7]);
    expect(updateFieldMock).toHaveBeenCalledWith("cast_members", [
      { employee_full_name: "new value", role_name: "Role One" }
    ]);
  });

  test("calls updateField when saving a cast member's role_name", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const saveButtons = await screen.findAllByTestId("save-button");
    fireEvent.click(saveButtons[8]);
    expect(updateFieldMock).toHaveBeenCalledWith("cast_members", [
      { employee_full_name: "Actor One", role_name: "new value" }
    ]);
  });

  test("renders cast member removal button and calls updateField on removal", async () => {
    const filmInfoWithTwoCast = {
      id: "film1",
      cast_members: [
        { employee_full_name: "Actor One", role_name: "Role One" },
        { employee_full_name: "Actor Two", role_name: "Role Two" },
      ],
    };
    const updateFieldMock = jest.fn();
    render(
      <TestComponent filmInfo={filmInfoWithTwoCast} updateField={updateFieldMock} />
    );
    const removalButtons = screen.getAllByRole("button", { name: "-" });
    expect(removalButtons.length).toBeGreaterThan(0);
    fireEvent.click(removalButtons[0]);
    expect(updateFieldMock).toHaveBeenCalledWith(
      "cast_members",
      [{ employee_full_name: "Actor Two", role_name: "Role Two" }]
    );
  });

  test("renders cast member addition button and calls updateField on addition", async () => {
    const updateFieldMock = jest.fn();
    render(<TestComponent updateField={updateFieldMock} />);
    const additionButton = screen.getByRole("button", { name: "+" });
    fireEvent.click(additionButton);
    expect(updateFieldMock).toHaveBeenCalledWith("cast_members", [
      { employee_full_name: "Actor One", role_name: "Role One" },
      { employee_full_name: "Actor", role_name: "Role" },
    ]);
  });

  test("disables editing when user is not ADMIN", async () => {
    render(<TestComponent userData={{ role: "USER" }} />);
    const saveButtons = screen.queryAllByTestId("save-button");
    expect(saveButtons).toHaveLength(0);
  });

  test("displays error message for invalid year", async () => {
    render(<YearErrorTestComponent />);
    await waitFor(() => {
      const errorMsg = screen.getByText(/Please enter a valid number/i);
      expect(errorMsg).toBeInTheDocument();
    });
  });
});
