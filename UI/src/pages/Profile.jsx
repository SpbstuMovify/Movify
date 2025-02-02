import React, { useEffect, useState } from 'react';
import './Profile.css';
import Navigation from '../components/general/Navigation';
import { useAuth } from '../contexts/AuthContext';
import { getUserById, changePassword, deleteUser } from '../services/api';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import ConfirmationModal from '../components/profile_components/ConfirmationModal';
import ChangePasswordForm from '../components/profile_components/ChangePasswordForm';
import ProfileInfo from '../components/profile_components/ProfileInfo';

function Profile() {
  const navigate = useNavigate();
  const { userData, setUserData, clearUserData } = useAuth();
  const {
    register,
    formState: { errors, isSubmitting, isSubmitSuccessful },
    handleSubmit,
    getValues,
    setError,
    clearErrors
  } = useForm();
  const [userInfo, setUserInfo] = useState(null);
  const [openedConfirmation, setOpenedConfirmation] = useState(false);
  const [loadingDelete, setLoadingDelete] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const fetchUserInfo = async () => {
      const info = await getUserById(userData.user_id, userData.token);
      setUserInfo(info);
    };
    if (userData) {
      fetchUserInfo();
    }
  }, [userData]);

  const handlePasswordChange = async (data) => {
    try {
      const response = await changePassword(
        data.password,
        userData.login,
        userData.email,
        userData.role,
        userData.token
      );
      setUserData({
        login: userData.login,
        email: userData.email,
        user_id: userData.user_id,
        role: userData.role,
        token: response.token
      });
    } catch (error) {
      switch (error.response?.status) {
        case 500:
          setError("root", { message: 'Server error occurred!' });
          break;
        default:
          setError("root", { message: 'There was an error changing the password...' });
      }
    }
  };

  const handleDeleteAccount = async () => {
    try {
      setLoadingDelete(true);
      await deleteUser(userData.user_id, userData.token);
      setLoadingDelete(false);
      clearUserData();
      navigate('/');
    } catch (error) {
      switch (error.response?.status) {
        case 401:
          clearUserData();
          break;
        default:
          console.error(error.message);
      }
      setErrorMessage("There was an error deleting the account!");
    }
  };

  const handleLogOut = () => {
    clearUserData();
    navigate('/');
  };

  return (
    <>
      <ConfirmationModal
        opened={openedConfirmation}
        onClose={() => setOpenedConfirmation(false)}
        onConfirm={handleDeleteAccount}
        loadingDelete={loadingDelete}
        errorMessage={errorMessage}
      />
      <Navigation />
      <div className="profile-body" role="profile-body">
        {userInfo && (
          <div className="profile-wrapper">
            <h1>{userInfo.login}</h1>
            <div style={{ display: "flex", justifyContent: "space-around", width: "100%" }}>
                <ProfileInfo userInfo={userInfo} />
                <ChangePasswordForm
                register={register}
                handleSubmit={handleSubmit}
                handlePasswordChange={handlePasswordChange}
                errors={errors}
                isSubmitting={isSubmitting}
                isSubmitSuccessful={isSubmitSuccessful}
                getValues={getValues}
                setError={setError}
                />
            </div>
            <div style={{ display: "flex", gap: "5vw" }}>
              <button className="quit-button delete-button" onClick={() => setOpenedConfirmation(true)}>
                Delete account
              </button>
              <button className="quit-button exit-button" onClick={handleLogOut}>
                Log out
              </button>
            </div>
          </div>
        )}
      </div>
    </>
  );
}

export default Profile;
