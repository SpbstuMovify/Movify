import React, { useState, useEffect } from "react";
import './DropdownMultiSelect.css'

const DropdownMultiSelect = ({ name, options, onChange, placeholder="Select options" }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState([]);

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };

  // Handle selection changes
  const handleOptionChange = (event) => {
    const value = event.target.value;
    const updatedOptions = selectedOptions.includes(value)
      ? selectedOptions.filter((option) => option !== value)
      : [...selectedOptions, value];

    setSelectedOptions(updatedOptions);
    onChange(updatedOptions); // Pass changes to the parent component (form)
  };

  // Close dropdown if clicked outside
  const handleOutsideClick = (event) => {
    if (!event.target.closest(".dropdown")) {
      setIsOpen(false);
    }
  };

  useEffect(() => {
    document.addEventListener("click", handleOutsideClick);
    return () => document.removeEventListener("click", handleOutsideClick);
  }, []);

  return (
    <div className="dropdown" style={{ position: "relative", width: "200px" }}>
      <button type="button" onClick={toggleDropdown}>
        <div>
        {selectedOptions.length > 0
          ? selectedOptions.join(", ")
          : placeholder}
        </div>
        <div>{isOpen ? "▲" : "▼"}</div>
      </button>

      {isOpen && (
        <div className="dropdown-content">
          {options.map((option) => (
            <label key={option}>
              <input
                type="checkbox"
                value={option}
                checked={selectedOptions.includes(option)}
                onChange={handleOptionChange}
                style={{ marginRight: "8px" }}
              />
              {option}
            </label>
          ))}
        </div>
      )}
      <input type="hidden" name={name} value={selectedOptions} />
    </div>
  );
};


export default DropdownMultiSelect;