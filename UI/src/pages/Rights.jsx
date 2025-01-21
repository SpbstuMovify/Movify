import './Profile.css';
import Navigation from '../components/Navigation';
import { useAuth } from '../contexts/AuthContext';
import { getUserByLogin, grandToAdmin } from '../services/api';
import { useForm } from 'react-hook-form';

function Rights() {
    const { userData } = useAuth();
    const {
        register,
        formState: { errors, isSubmitting, isSubmitSuccessful },
        handleSubmit,
        setError,
        reset,
    } = useForm();

    const handleGrantToAdmin = async (data) => {
        try {
            const user = await getUserByLogin(data.login, userData.token);

            if (!user || !user.user_id) {
                setError("login", { message: 'User not found!' });
                return;
            }

            console.log('User found:', user);

            await grandToAdmin(user.user_id, userData.token);

            console.log('Admin rights successfully granted');
            reset(); // Сброс формы после успешного выполнения
        } catch (error) {
            console.error('Error granting admin rights:', error);

            const errorMessage =
                error.response?.status === 500
                    ? 'Server error occurred!'
                    : 'An error occurred while granting admin rights.';

            setError("root", { message: errorMessage });
        }
    };

    return (
        <>
            <Navigation />
            <div className="profile-body">
                <div className="profile-wrapper">
                    <div style={{ display: "flex", justifyContent: "space-around", width: "100%" }}>
                        <form
                            onSubmit={handleSubmit(handleGrantToAdmin)}
                            style={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                flexDirection: "column",
                            }}
                        >
                            <h3><b>Grant Administrator Rights</b></h3>
                            <input
                                {...register("login", {
                                    required: "Please enter the login!",
                                    pattern: {
                                        value: /^[A-Za-z]+[A-Za-z0-9_,.-]*$/,
                                        message: "Login must start with a Latin letter",
                                    },
                                })}
                                placeholder="Login"
                                type="text"
                            />
                            {errors.login && (
                                <label
                                    htmlFor="login"
                                    className="error-message error-message-small"
                                >
                                    {errors.login.message}
                                </label>
                            )}
                            <button type="submit" disabled={isSubmitting}>
                                {isSubmitting ? 'Processing...' : 'Submit'}
                            </button>
                            {isSubmitting && (
                                <p className="loading-wrapper">
                                    <img src="/images/loading.gif" alt="Loading..." />
                                </p>
                            )}
                            {isSubmitSuccessful && (
                                <p className="response-message success-message">
                                    Admin rights successfully granted!
                                </p>
                            )}
                            {errors.root && (
                                <p className="response-message error-message">
                                    {errors.root.message}
                                </p>
                            )}
                        </form>
                    </div>
                </div>
            </div>
        </>
    );
}

export default Rights;
