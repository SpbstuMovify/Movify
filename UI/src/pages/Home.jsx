// src/pages/home.js
import {React} from 'react';
import './Home.css'

function Home() {
    return <div className="homeScreen">
        <div className="homeScreenGradient" />
        <div className="homeScreenBody">
            <>
                <h1>Unlimited movies, TV shows, and more</h1>
                <h2>Watch anywhere. Cancel at any time.</h2>
                <h3>Ready to watch?</h3>
                <div className="homeScreenInput">
                    <button className="homeScreenGetStarted">
                        Get Started
                    </button>
                </div>
            </>
        </div>
    </div>;
}

export default Home;
