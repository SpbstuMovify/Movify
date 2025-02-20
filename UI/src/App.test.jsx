import { render, screen } from "@testing-library/react"
import { RouterProvider, createMemoryRouter, createBrowserRouter } from "react-router-dom";
import '@testing-library/jest-dom'
import routesConfig from "./configs/routesConfig";
import App from './App.jsx';

jest.clearAllMocks();
jest.mock('@services/api', () => require('./__mocks__/services/api'));

describe('App component', () => {
      
      test("renders App with RouterProvider", () => {
        const { container } = render(<App />);
        expect(container.querySelector(".app")).toBeInTheDocument();
      });
})

describe('Routing of unprotected components', () => {
    test('renders Home page at root "/"', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        const homeText = await screen.findByText(/Unlimited movies, TV shows, and more/i);
        expect(homeText).toBeInTheDocument();
    });
  
    test('renders Sign In page at "/login"', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/login"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        const signInText = await screen.findByText(/New to Movify\?/i);
        expect(signInText).toBeInTheDocument();
    });

    test('renders Register page at "/register"', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/register"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        const signInText = await screen.findByText(/Already have an account\?/i);
        expect(signInText).toBeInTheDocument();
    });

    test('renders Films page at "/films"', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/films"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        const signInText = await screen.findByText(/Watch all movies and series you want!/i);
        expect(signInText).toBeInTheDocument();
    });

    test('renders FilmDetail page at "/films/{id}"', async () => {
        const router = createMemoryRouter(routesConfig, {
            initialEntries: ["/films/id"]
        })
        render(<RouterProvider future={{
            v7_relativeSplatPath: true,
            v7_startTransition: true
          }} router={router}/>);
        const signInText = await screen.findByRole("detail-body", {});
        expect(signInText).toBeInTheDocument();
    });
  
});