import { render, screen } from "@testing-library/react"
import '@testing-library/jest-dom'
import App from "./App" 

describe('App', () => { 
    test('renders correctly', () => {
        render(<App />);
        const textElement = screen.getByText("Unlimited movies, TV shows, and more");
        expect(textElement).toBeInTheDocument();
    })
})