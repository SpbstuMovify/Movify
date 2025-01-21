import React from 'react'
import './SignInScreen.css'
import { useForm } from "react-hook-form"
import {login as loginUser} from "../services/api" 
import {Link, useNavigate} from "react-router-dom";
import { useAuth } from '../contexts/AuthContext';

function Login() {
    const { register, handleSubmit, formState: {errors, isSubmitting, isSubmitSuccessful}, setError } = useForm();
    const navigate = useNavigate();
    const { setUserData } = useAuth();

    const handleLogin = async (data) => {
        try {
            const ipResponse = await fetch('https://api.ipify.org?format=json');
            const ipData = await ipResponse.json();
            const userData = await loginUser(data.login, data.password, ipData.ip);
            localStorage.setItem('userData', JSON.stringify(userData));
            setUserData(userData)
            navigate('/films');
            } catch (error) {
                switch (error.response?.status) {
                    case 401:
                        setError("root", {
                            message: error.response.data.body.detail == 'Please try again' 
                            ? 'Incorrect login or password!' : 'Max amount of attempts reached!'
                        });
                        break;

                    case 404: 
                        setError("root", {
                            message: 'Incorrect login or password!'
                        });
                        break;
    
                    case 500:
                        setError("root", {
                            message: 'Server error occurred!'
                        });
                        break;
    
                    default:
                        setError("root", {
                            message: 'There was an error logging in!'
                        });
                }
            }
    };
    return (
        <div className="signInScreen">
            <div className="signInNav">
                <Link className='nav-sign-link' to="/">
                    <img
                        className="signInScreenLogo"
                        src="/images/movify_logo_red.png"
                        alt=""
                    />
                </Link>
                <button onClick={() => navigate("/register")} className="signUpScreenButton">
                    Sign Up
                </button>
            </div>
                <div className="sign-body">
                <form onSubmit={handleSubmit(handleLogin)}>
                    <h1>Sign In</h1>
                    <input {...register("login", {
                        required: "Please enter the login or email!",
                    })} placeholder="Login or email" type="text"/>
                    {errors.login && (<label htmlFor='login' className="error-message error-message-small">
                        {errors.login.message}</label>)}

                    <input {...register("password", {
                        required: "Please enter the password!"
                    })} placeholder="Enter the password" type="password" />
                    {errors.password && (<label htmlFor='password' className="error-message error-message-small">
                        {errors.password.message}</label>)}

                    <button type="submit" disabled={isSubmitting}>Sign in</button>
                    {isSubmitting ? <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
                        : isSubmitSuccessful ? 
                        <p className='response-message'>Login successful!</p> 
                        : <p className='error-message'>{errors.root && (errors.root.message)}</p>}
                    <h4>
                        <span className="bodyGray">New to Movify? </span>
                        <span className="bodyLink" onClick={() => navigate("/register")}>Sign Up now.</span>
                    </h4>
                </form>
            </div>
        </div>
    );
}

export default Login;
