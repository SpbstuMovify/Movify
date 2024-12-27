import React from 'react'
import './SignInScreen.css'
import { useForm } from "react-hook-form"
import {Link, useNavigate} from "react-router-dom";

function Login() {
    const { register, handleSubmit, formState: {errors, isSubmitting}, setError } = useForm();
    const navigate = useNavigate();
    //const {login} = useContext(AuthContext);

    const handleLogin = async (data) => {
        try {
            //await login(data.login, data.password);
            navigate('/films');
            } catch (error) {
                switch (error.response?.status) {
                    case 400:
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
                        pattern: {
                            value: /^[A-Za-z0-9_,.-@]*$/,
                            message: "Login must consist of latin letters, numbers and special symbols"
                        },
                        minLength: {
                            value: 6,
                            message: "Login or email must be at least 6 symbols long",
                        },
                    })} placeholder="Login or email" type="text"/>
                    {errors.login && (<label htmlFor='login' className="error-message error-message-small">
                        {errors.login.message}</label>)}

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

                    <button type="submit" disabled={isSubmitting}>Sign in</button>
                    {isSubmitting ? <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
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

/*
function Login() {
    const navigate = useNavigate();
    const {login} = useContext(AuthContext);

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        setErrorMessage('');
        try {
            await login(email, password);
            
        } catch (error) {
            switch (error.response?.status) {
                case 400:
                    setErrorMessage('Incorrect login or password!');
                    break;

                case 500:
                    setErrorMessage('Server error occurred!');
                    break;

                default:
                    setErrorMessage('There was an error logging in!');
            }
            console.log(error.response?.status);
        }
        setLoading(false);
    };
    return (
        <div className="signInScreen">
            <div className="signInNav">
                <Link to="/">
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
                <form onSubmit={handleLogin}>
                    <h1>Sign In</h1>
                    <input
                        placeholder="Email"
                        type="email"
                        value={email}
                        required={true}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <input
                        placeholder="Password"
                        type="password"
                        required={true}
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <button type="submit">Sign In</button>
                    {loading ? <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
                    : <p className='error-message'>{errorMessage}</p>}
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
*/