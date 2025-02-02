import React from 'react';
import '../../pages/Profile.css';

const ConfirmationModal = ({ opened, onClose, onConfirm, loadingDelete, errorMessage }) => {
  if (!opened) return null;

  return (
    <>
      <div className="black-background" />
      <div className="confirmation-window">
        <div className="button-wrapper">
          <h2 id="confirmation-header">Are you sure you want to delete your account?</h2>
          <button onClick={onClose}>X</button>
        </div>
        {loadingDelete ? (
          <p className="loading-wrapper">
            <img src="/images/loading.gif" alt="loading" />
          </p>
        ) : (
          <p className="response-message error-message">{errorMessage}</p>
        )}
        <div className="confirmation-wrapper">
          <button onClick={onConfirm} className="confirm-button">Yes</button>
          <button onClick={onClose} className="no-button">No</button>
        </div>
      </div>
    </>
  );
};

export default ConfirmationModal;
