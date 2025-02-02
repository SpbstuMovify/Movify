import React from 'react';
import '../../pages/Profile.css';

const ProfileInfo = ({ userInfo }) => {
  return (
    <div className="profile-info">
      <div style={{ display: "flex", justifyContent: "space-around", width: "100%" }}>
        <div>
          <h3><b>Email:</b> {userInfo.email}</h3>
          <h3><b>First name:</b> {userInfo.first_name}</h3>
          <h3><b>Last name:</b> {userInfo.last_name}</h3>
        </div>
      </div>
    </div>
  );
};

export default ProfileInfo;
