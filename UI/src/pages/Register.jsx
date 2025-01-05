// src/pages/Register.js
import React, { useState } from 'react';
import './Register.css'
import {Link, useNavigate} from "react-router-dom";
import {register as registerUser} from "../services/api";
import { useForm, Controller } from "react-hook-form";
import DropdownMultiSelect from '../components/DropdownMultiSelect';
import { useAuth } from '../contexts/AuthContext';

const Register = () => {
    const { setUserData } = useAuth();
    const navigate = useNavigate();
    const { register, control, handleSubmit, formState: {errors, isSubmitting, isSubmitSuccessful}, 
    getValues, setError, clearErrors } = useForm();

    const [openedTerms, setOpenedTerms] = useState(false);
    const [terms, setTerms] = useState('');
    
    const handleRegister = async (data) => {
        try {
            const userData = await registerUser(data.email, data.password, data.login, data.firstName, data.lastName);
            localStorage.setItem('userData', JSON.stringify(userData));
            setUserData(userData);
            navigate('/films');
        } catch (error) {
            switch (error.response?.status) {
                case 400:
                    setError("root", {
                        message: 'User with this login and password already exists!'
                    });
                    break;

                case 500:
                    setError("root", {
                        message: 'Server error occurred!'
                    });
                    break;

                default:
                    setError("root", {
                        message: 'There was an error creating an account!'
                    });
            }
        }
    };

    const handleOpenTerms = async () => {
        try {
          const response = await fetch('/terms.txt');  // Fetch the file from the public folder
          if (!response.ok) {
            throw new Error('File not found');
          }
          const text = await response.text(); // Read the file content as text
          setTerms(text); // Set the file content in the state
        } catch (error) {
          console.error('Error reading file:', error);
          setTerms('Failed to load the file.');
        }
        setOpenedTerms(true);
      };

    return <div className="signUpScreen">
        {openedTerms ? <div className="terms-window">
            <h2 id="terms-header">Movify Terms of Service</h2>
            <div className='button-wrapper'>
                <button onClick={()=>{setOpenedTerms(false)}}>X</button>
            </div>
            <p>{terms}</p>
        </div> : ''}
            <div className="signInNav">
                <Link className='nav-sign-link' to="/">
                    <img
                        className="signInScreenLogo"
                        src="/images/movify_logo_red.png"
                        alt=""
                    />
                </Link>
                <button onClick={() => navigate("/login")} className="signUpScreenButton">
                    Sign In
                </button>
        </div>
        <div className="sign-body">
            <form onSubmit={handleSubmit(handleRegister)}>
                <h2>Create an account and watch your favorite films any day!</h2>
                <div className='input-container'>
                    <div>
                        <input {...register("login", {
                            required: "Please enter the login!",
                            pattern: {
                                value: /^[A-Za-z]+[A-Za-z0-9_,.-]*$/,
                                message: "Login must start from a latin letter"
                            },
                            minLength: {
                                value: 6,
                                message: "Login must be from 6 to 32 symbols long",
                            },
                            maxLength: {
                                value: 32,
                                message: "Login must be from 6 to 32 symbols long",
                            },
                        })} placeholder="Login" type="text"/>
                        {errors.login && (<label htmlFor='login' className="error-message error-message-small">
                            {errors.login.message}</label>)}
                    </div>

                    <div>
                        <input {...register("email", {
                            required: "Please enter the email!",
                            pattern: {
                                value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                                message: "Incorrect email! Example: email@email.com"
                            }
                        })} placeholder="Email" type="email" />
                        {errors.email && (<label htmlFor='email' className="error-message error-message-small">
                            {errors.email.message}</label>)}
                    </div>

                    <div> 
                        <input {...register("password", {
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
                                else {
                                    clearErrors("repeatedPassword");
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
                        })} placeholder="Enter the password" type="password" />
                        {errors.password && (<label htmlFor='password' className="error-message error-message-small">
                            {errors.password.message}</label>)}
                    </div>

                    <div>
                        <input {...register("repeatedPassword", {
                            required: "Please repeat the password",
                            validate: (value) => {
                                const values = getValues();
                                if (value !== values.password) {
                                return "Passwords do not match";
                                }
                                return true;
                            },
                        })} placeholder="Repeat the password" type="password" />
                        {errors.repeatedPassword && (<label htmlFor='repeatedPassword' className="error-message error-message-small">
                            {errors.repeatedPassword.message}</label>)}
                    </div>
                    
                    <div>
                        <input {...register("firstName", {
                            required: false,
                        })} placeholder="Enter first name (optional)" type="text" />
                    </div>

                    <div>
                        <input {...register("lastName", {
                            required: false,
                        })} placeholder="Enter last name (optional)" type="text" />
                    </div>
                </div>
                <div id="preference-list-wrapper">
                    <Controller
                        name="preferences"
                        control={control}
                        render={({ field: { value, onChange } }) => (
                        <DropdownMultiSelect
                            name="preferences"
                            options={["Action", "Comedy", "Crime", "Drama", "Fantasy", "Sci-Fi"]}
                            value={value}
                            onChange={onChange}
                            placeholder='Select preferences'
                        />
                        )}
                    />
                </div>
                <div>
                    <input className={errors.termsAgreed ? "custom-checkbox error-checkbox" : "custom-checkbox"} 
                    {...register("termsAgreed", {required: true})} type="checkbox" id="terms" />
                    <label style={{userSelect: "none"}} htmlFor="terms">I agree to </label> 
                    <button type="button" className='terms-button' onClick={()=>{handleOpenTerms()}}>
                        Movify Terms of Service</button>
                </div>
                <button type="submit" disabled={isSubmitting}>Create</button>
                {isSubmitting ? 
                    <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
                    : isSubmitSuccessful ? 
                        <p className='response-message'>Registration successful!</p> 
                        : <p className='response-message error-message'>{errors.root && (errors.root.message)}</p>}
                <h4 style={{marginTop: "0"}}>
                    <span className="bodyGray">Already have an account? </span>
                    <span className="bodyLink" onClick={() => navigate("/login")}>Click here to sign in.</span>
                </h4>
            </form>
        </div>
    </div>
};

export default Register;
