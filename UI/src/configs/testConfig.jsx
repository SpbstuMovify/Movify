import { MemoryRouter } from "react-router-dom";
import { render } from "@testing-library/react"
export const renderWithRouter = (ui) => {
    return render(<MemoryRouter future={{
        v7_relativeSplatPath: true,
        v7_startTransition: true
    }}>{ui}</MemoryRouter>);
};