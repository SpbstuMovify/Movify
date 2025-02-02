import React from "react";

const Pagination = ({
  pageNumber,
  maxPageNumber,
  onPageBack,
  onPageForward,
  onJumpFirst,
  onJumpLast,
  pageSize,
  onPageSizeChange,
  pageSizeOptions,
}) => {
  return (
    <div className="page-buttons">
      <button disabled={pageNumber === 0} className="page-button" onClick={onPageBack}>
        {"<"}
      </button>
      <div>
        <button
          style={{ marginRight: "15px" }}
          disabled={pageNumber === 0}
          className="page-button"
          onClick={onJumpFirst}
        >
          {"<<"}
        </button>
        <span>
          Page: {pageNumber + 1} / {maxPageNumber + 1}&nbsp;&nbsp;&nbsp;
          Films on page:{" "}
          <select
            value={pageSize}
            className="page-size-dropdown"
            onChange={onPageSizeChange}
          >
            {pageSizeOptions.map((size) => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
        </span>
        <button
          style={{ marginLeft: "15px" }}
          disabled={pageNumber === maxPageNumber}
          className="page-button"
          onClick={onJumpLast}
        >
          {">>"}
        </button>
      </div>
      <button disabled={pageNumber === maxPageNumber} className="page-button" onClick={onPageForward}>
        {">"}
      </button>
    </div>
  );
};

export default Pagination;