// src/components/Navigation.js
import React from 'react';
import { Link } from 'react-router-dom';
import './Navigation.css'
import { useAuth } from '../contexts/AuthContext';

const Navigation = () => {
    const { userData } = useAuth();
    return <div className="nav nav-black">
        <div className="nav-contents">
            <Link className='nav-logo-link' to="/">
                <img
                    className="nav-logo"
                    src="../images/movify_logo_red.png"
                    alt=""
                />
            </Link>
            <div className="nav-container">
                {userData && userData.role === "ADMIN" && <Link className="nav-text-links" to="/rights">
                    Rights
                </Link>}
                <Link className="nav-text-links" to="/films">
                    Films
                </Link>
                {userData && <Link className="nav-text-links" to="/favorites">
                    Favorites
                </Link>}
                {userData ?
                    <Link className="nav-text-links" to="/profile">
                        {userData.login}
                    </Link>
                    : <Link className="nav-text-links" to="/login">
                        Sign in
                    </Link>}
            </div>
        </div>
    </div>
};

export default Navigation;
