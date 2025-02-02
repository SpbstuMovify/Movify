import './Home.css'
import Navigation from '../components/general/Navigation';
import { useNavigate } from 'react-router-dom';

function Home() {
    const navigate = useNavigate();
    return <div className="homeScreen">
        <Navigation />
        <div className="homeScreenGradient" />
        <div className="homeScreenBody">
            <div className="home-label-container">
                <h1>Unlimited movies, TV shows, and more</h1>
                <h2>Watch anywhere. Cancel at any time.</h2>
                <h3>Ready to watch?</h3>
                <div className="homeScreenInput">
                    <button className="homeScreenGetStarted" onClick={()=>{navigate('/films')}}>
                        Get Started
                    </button>
                </div>
            </div>
        </div>
    </div>;
}

export default Home;
