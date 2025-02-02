import { AuthProvider } from '../contexts/AuthContext.jsx';
import Home from '../pages/Home.jsx';
import SignInScreen from '../pages/SignInScreen.jsx';
import Register from '../pages/Register.jsx';
import Films from '../pages/Films.jsx';
import FavoriteFilms from '../pages/FavoriteFilms.jsx';
import Profile from '../pages/Profile.jsx';
import ProtectedRoute from '../components/general/ProtectedRoute.jsx';
import FilmDetail from '../pages/FilmDetail.jsx';
import Rights from '../pages/Rights.jsx';

const routesConfig = [
    {
      element: <AuthProvider />,
      children: [
        { path: "/", element: <Home /> },
        { path: "/login", element: <SignInScreen /> },
        { path: "/register", element: <Register /> },
        { path: "/films", element: <Films /> },
        { path: "/films/:contentId", element: <FilmDetail /> },
        {
          element: <ProtectedRoute />,
          children: [
            { path: "/profile", element: <Profile /> },
            { path: "/favorites", element: <FavoriteFilms /> },
          ]
        },
        {
          element: <ProtectedRoute checkAdmin={true} />,
          children: [
            { path: "/rights", element: <Rights /> },
          ]
        },
      ]
    }
];

export default routesConfig;