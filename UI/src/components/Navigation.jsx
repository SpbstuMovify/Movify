// src/components/Navigation.js
import React from 'react';
import { Link } from 'react-router-dom';
import './Navigation.css'

const Navigation = () => {
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
                <Link className="nav-text-links" to="/films">
                    Films
                </Link>
                <Link className="nav-text-links" to="/favorites">
                    Favorites
                </Link>
                <Link className="nav-text-links" to="/login">
                    Sign in
                </Link>
            </div>
        </div>
    </div>
};

export default Navigation;
