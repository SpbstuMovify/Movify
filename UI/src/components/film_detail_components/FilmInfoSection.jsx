import React from "react";
import { Controller } from "react-hook-form";
import EditableField from "../general/EditableField";
import mappingJSON from "../../configs/config";
import "../../pages/FilmDetail.css"

const FilmInfoSection = ({ filmInfo, filmForm, userData, updateField, currentYear }) => {
  if (!filmInfo) return null;
  return (
    <div className="film-detail-info">
      <h1 className="film-detail-title">
        <Controller
          name="title"
          control={filmForm.control}
          render={({ field }) => (
            <EditableField
              label=""
              value={field.value}
              onSave={(newValue) => updateField("title", newValue)}
              onChange={field.onChange}
              disableEditButton={!userData || userData.role !== "ADMIN"}
            />
          )}
        />
      </h1>
      <Controller
        name="genre"
        control={filmForm.control}
        render={({ field, fieldState }) => (
          <>
            <EditableField
              label="Genre:"
              value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
              onSave={(newValue) => updateField("genre", newValue, fieldState.error)}
              onChange={field.onChange}
              disableEditButton={!userData || userData.role !== "ADMIN"}
              inputType="select"
              options={mappingJSON().genre_options}
              selectDisplayedValue={field.value}
            />
            {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
          </>
        )}
      />
      <Controller
        name="category"
        control={filmForm.control}
        render={({ field, fieldState }) => (
          <>
            <EditableField
              label="Category:"
              value={field.value && field.value.toUpperCase().replace(/ /g, "_")}
              onSave={(newValue) => updateField("category", newValue, fieldState.error)}
              onChange={field.onChange}
              disableEditButton={!userData || userData.role !== "ADMIN"}
              inputType="select"
              options={mappingJSON().category_options}
              selectDisplayedValue={field.value}
            />
            {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
          </>
        )}
      />
      <Controller
        name="age_restriction"
        control={filmForm.control}
        render={({ field, fieldState }) => (
          <>
            <EditableField
              label="Age restriction:"
              value={field.value}
              onSave={(newValue) => updateField("age_restriction", newValue, fieldState.error)}
              onChange={field.onChange}
              disableEditButton={!userData || userData.role !== "ADMIN"}
              inputType="select"
              options={mappingJSON().age_restriction_options}
              selectDisplayedValue={mappingJSON().age_restriction[field.value]}
            />
            {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
          </>
        )}
      />
      <Controller
        name="publisher"
        control={filmForm.control}
        render={({ field }) => (
          <EditableField
            label="Publisher:"
            value={field.value}
            onSave={(newValue) => updateField("publisher", newValue)}
            onChange={field.onChange}
            disableEditButton={!userData || userData.role !== "ADMIN"}
          />
        )}
      />
      <Controller
        name="year"
        control={filmForm.control}
        rules={{
          validate: (value) => {
            if (isNaN(value)) {
              return "Please enter a valid number";
            }
            if (value < 1900 || value > currentYear) {
              return `Please enter a year between 1900 and ${currentYear}`;
            }
            return true;
          },
        }}
        render={({ field, fieldState }) => (
          <>
            <EditableField
              label="Release year:"
              value={field.value}
              onSave={(newValue) => updateField("year", newValue, fieldState.error)}
              onChange={field.onChange}
              disableEditButton={!userData || userData.role !== "ADMIN"}
              inputType="number"
              maxNumber={currentYear}
              minNumber={1900}
            />
            {fieldState.error && <p style={{ color: "red" }}>{fieldState.error.message}</p>}
          </>
        )}
      />
      <Controller
        name="description"
        control={filmForm.control}
        render={({ field }) => (
          <EditableField
            label="Description:"
            value={field.value}
            onSave={(newValue) => updateField("description", newValue)}
            onChange={field.onChange}
            disableEditButton={!userData || userData.role !== "ADMIN"}
            inputType={"textarea"}
          />
        )}
      />
      {filmInfo.cast_members && filmInfo.cast_members.length !== 0 && (
        <div style={{ display: "flex" }}>
          <b style={{ marginRight: "15px", whiteSpace: "nowrap" }}>Cast members:</b>
          <div>
            {filmInfo.cast_members.map((member, index) => (
              <div key={index} style={{ marginBottom: "10px" }}>
                {userData && userData.role === "ADMIN" && filmInfo.cast_members.length !== 1 && (
                  <button
                    className="cast-member-button"
                    style={{ marginRight: "7px" }}
                    onClick={() =>
                      updateField("cast_members", filmInfo.cast_members.filter((m) => m !== member))
                    }
                  >
                    -
                  </button>
                )}
                <Controller
                  name={`cast_members.${index}.employee_full_name`}
                  control={filmForm.control}
                  render={({ field }) => (
                    <EditableField
                      label=""
                      value={field.value}
                      onSave={(newValue) => {
                        const updatedCast = filmForm.getValues("cast_members");
                        updatedCast[index] = { ...updatedCast[index], employee_full_name: newValue };
                        updateField("cast_members", updatedCast);
                      }}
                      onChange={field.onChange}
                      disableEditButton={!userData || userData.role !== "ADMIN"}
                      isPTag={false}
                    />
                  )}
                />
                <Controller
                  name={`cast_members.${index}.role_name`}
                  control={filmForm.control}
                  render={({ field }) => (
                    <EditableField
                      label=" - "
                      value={field.value}
                      onSave={(newValue) => {
                        const updatedCast = filmForm.getValues("cast_members");
                        updatedCast[index] = { ...updatedCast[index], role_name: newValue };
                        updateField("cast_members", updatedCast);
                      }}
                      onChange={field.onChange}
                      disableEditButton={!userData || userData.role !== "ADMIN"}
                      isPTag={false}
                    />
                  )}
                />
                {userData &&
                  userData.role === "ADMIN" &&
                  filmInfo.cast_members[filmInfo.cast_members.length - 1] === member && (
                    <button
                      style={{ marginLeft: "7px" }}
                      className="cast-member-button"
                      onClick={() =>
                        updateField("cast_members", [
                          ...filmInfo.cast_members,
                          { employee_full_name: "Actor", role_name: "Role" },
                        ])
                      }
                    >
                      +
                    </button>
                  )}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default FilmInfoSection;