import React, { useState } from "react";

const EditableField = ({ label, 
  value,
  onSave, 
  onChange, 
  disableEditButton = true, 
  inputType = "input", 
  isPTag = true,
  options = [],
  maxNumber = 3000,
  minNumber = 0,
  selectDisplayedValue = value
 }) => {
  const [isEditing, setIsEditing] = useState(false);
  const maxInputSize = 50;

  const handleSave = (value) => {
      setIsEditing(false);
      onSave(value);
  };

  const renderInput = () => {
    switch (inputType)
    {
      case "input":
        return(<input
          type="text"
          value={value}
          onChange={(e) => {onChange(e.target.value)}}
          style={{ marginLeft: "10px" }}
          size={value.toString().length < maxInputSize ? value.toString().length : maxInputSize}
        />)

      case "number":
        return(<input
          type="number"
          max={maxNumber}
          min={minNumber}
          value={value}
          onChange={(e) => {onChange(e.target.value)}}
          style={{ marginLeft: "10px" }}
          size={value.toString().length < maxInputSize ? value.toString().length : maxInputSize}
        />)

      case "textarea":
        return (<textarea
          value={value}
          onChange={(e) => {onChange(e.target.value)}}
          style={{ marginLeft: "10px", width: "70%", minHeight: "70px" }}
        />);

      case "select":
        return <select
          value={value}
          onChange={(e) => onChange(e.target.value)}
          style={{ marginLeft: "10px" }}
        >
          {options.map((option, index) => (
            <option value={option.value} key={index}>
              {option.label}
            </option>
          ))}
        </select>

    }
    throw new Error("Invalid input type");
  }

  const elements = (
      <>
          <b style={{ whiteSpace: "nowrap" }}>{label}</b>
          {isEditing ? (
              <>
                  {renderInput()}
                  <button
                      onClick={(e)=>handleSave(value)}
                      className="edit-image-button"
                      style={{ marginLeft: "10px", border: "none" }}
                  >
                      <img src="/images/border_checkmark.png" />
                  </button>
              </>
          ) : (
              <>
                  <span style={label === "" ? { marginLeft: "0" } : { marginLeft: "10px" }}>{inputType=="select" ? selectDisplayedValue : value}</span>
                  <button
                      hidden={disableEditButton}
                      onClick={() => setIsEditing(true)}
                      className="edit-image-button"
                      style={{ marginLeft: "10px", border: "none" }}
                  >
                      <img src="/images/pencil.png" />
                  </button>
              </>
          )}
      </>
  );

  return isPTag ? <p style={{ marginTop: "0", display: "flex"}}>{elements}</p> : <>{elements}</>;
};

export default EditableField;