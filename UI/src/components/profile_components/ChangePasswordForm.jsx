import React from 'react';
import '../../pages/Profile.css';

const ChangePasswordForm = ({
  register,
  handleSubmit,
  handlePasswordChange,
  errors,
  isSubmitting,
  isSubmitSuccessful,
  getValues,
  setError
}) => {
  return (
    <form
      onSubmit={handleSubmit(handlePasswordChange)}
      style={{ display: "flex", justifyContent: "center", alignItems: "center", flexDirection: "column" }}
    >
      <h3><b>Change password</b></h3>
      <input
        {...register("password", {
          required: "Please enter the password!",
          validate: (value) => {
            if (!/\d/.test(value)) {
              return "Password should include at least one number";
            }
            if (!/[!@#$%^&*(),.?":{}|<>_]/.test(value)) {
              return "Password should include at least one special symbol";
            }
            if (!/[a-z]/.test(value)) {
              return "Password should include at least one lowercase latin letter";
            }
            if (!/[A-Z]/.test(value)) {
              return "Password should include at least one uppercase latin letter";
            }
            const values = getValues();
            if (value !== values.repeatedPassword) {
                setError("repeatedPassword", {
                    message: "Passwords do not match"
                });
            }
            return true;
          },
          minLength: {
            value: 6,
            message: "Password must be from 6 to 32 symbols long",
          },
          maxLength: {
            value: 32,
            message: "Password must be from 6 to 32 symbols long",
          },
        })}
        placeholder="Enter the password"
        type="password"
      />
      {errors.password && (
        <label className="error-message error-message-small">
          {errors.password.message}
        </label>
      )}
      <input
        {...register("repeatedPassword", {
          required: "Please repeat the password",
          validate: (value, formValues) => {
            if (value !== formValues.password) {
              return "Passwords do not match";
            }
            return true;
          },
        })}
        placeholder="Repeat the password"
        type="password"
      />
      {errors.repeatedPassword && (
        <label className="error-message error-message-small">
          {errors.repeatedPassword.message}
        </label>
      )}
      <button type="submit">Submit</button>
      {isSubmitting ? (
        <p className="loading-wrapper">
          <img src="/images/loading.gif" alt="loading" />
        </p>
      ) : isSubmitSuccessful ? (
        <p className="response-message">Password was changed!</p>
      ) : (
        <p className="response-message error-message">
          {errors.root && errors.root.message}
        </p>
      )}
    </form>
  );
};

export default ChangePasswordForm;
