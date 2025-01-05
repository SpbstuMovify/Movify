import './Profile.css'
import Navigation from '../components/Navigation';
import { useAuth } from '../contexts/AuthContext';
import { useEffect, useState } from 'react';
import { getUserById, changePassword, deleteUser } from '../services/api';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';

function Profile() {
    const navigate = useNavigate();
    const {userData, setUserData, clearUserData} = useAuth();
    const {register, formState: {errors, isSubmitting, isSubmitSuccessful}, 
    handleSubmit, getValues, setError, clearErrors} = useForm();
    const [userInfo, setUserInfo] = useState();
    const [openedConfirmation, setOpenedConfirmation] = useState(false);
    const [loadingDelete, setLoadingDelete] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");

    useEffect(()=>{
        const fetchUserInfo = async () => {
            setUserInfo(await getUserById(userData.user_id));
        }
        if (userData) {
            fetchUserInfo();
        }
    }, [userData]);

    const handlePasswordChange = async (data) => {
        try {
            const response = await changePassword(data.password, userData.login, userData.email, userData.role);
            setUserData({
                "login": userData.login,
                "email": userData.email,
                "user_id": userData.user_id,
                "role": userData.role,
                "token": response.token
            });
        } 
        catch (error) {
            switch (error.response?.status) {
                case 500:
                    setError("root", {
                        message: 'Server error occurred!'
                    });
                    break;

                default:
                    setError("root", {
                        message: 'There was an error changing the password...'
                    });
            }
        }
    }

    const handleDeleteAccount = async () => {
        try {
            setLoadingDelete(true);
            deleteUser(userData.user_id, userData.token);
            setLoadingDelete(false);
            clearUserData();
            navigate('/');
        }
        catch (error)
        {
            setErrorMessage("There was an error deleting the account!");
        }
    }

    const handleLogOut = async () => {
        clearUserData();
        navigate('/')
    }

    return <>
        {openedConfirmation ? <>
        <div className='black-background'/>
        <div className="confirmation-window">
            <div className='button-wrapper'>
                <h2 id="confirmation-header">Are you sure you want to delete your account?</h2>
                <button onClick={()=>{setOpenedConfirmation(false)}}>X</button>
            </div>
            {loadingDelete ? 
                    <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
                    : <p className='response-message error-message'>{errorMessage}</p>}
            <div className='confirmation-wrapper'>
                <button onClick={()=>{handleDeleteAccount()}} className='confirm-button'>Yes</button>
                <button onClick={()=>{setOpenedConfirmation(false)}} className='no-button'>No</button>
            </div>
        </div></> : ''}
        <Navigation />
        <div className="profile-body">
            { userInfo &&
            <div className='profile-wrapper'>
                <h1>{userInfo.login}</h1>
                <div style={{display: "flex", justifyContent: "space-around", width: "100%"}}>
                    <div>
                        <h3><b>Email:</b> {userInfo.email}</h3>
                        <h3><b>First name:</b> {userInfo.first_name}</h3>
                        <h3><b>Last name:</b> {userInfo.last_name}</h3>
                    </div>
                    <form onSubmit={handleSubmit(handlePasswordChange)}
                    style={{display: "flex", justifyContent: "center", alignItems: "center", flexDirection: "column"}}>
                        <h3><b>Change password</b></h3>
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
                        <button>Submit</button>
                        {isSubmitting ? 
                        <p className='loading-wrapper'><img src="/images/loading.gif"></img></p> 
                        : isSubmitSuccessful ? 
                            <p className='response-message'>Password was changed!</p> 
                            : <p className='response-message error-message'>{errors.root && (errors.root.message)}</p>}
                    </form>
                </div>
                <div style={{display: "flex", gap: "5vw"}}>
                <button className='quit-button delete-button' onClick={()=>{setOpenedConfirmation(true)}}>Delete account</button>
                <button className='quit-button exit-button' onClick={()=>{handleLogOut()}}>Log out</button>
                </div>
            </div>
            }
        </div>
    </>;
}

export default Profile;