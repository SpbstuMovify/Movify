import React from 'react';
import { render, act, screen, waitFor } from '@testing-library/react';
import { useFilms } from './useFilms'; // Adjust the import path as needed
import { searchFilms } from '../services/api'; // Uses your manual mock
import { searchFilmsResponse } from '../__mocks__/services/api'
import mappingJSON from '../configs/config';
import '@testing-library/jest-dom';

jest.mock('../services/api', () => require("../__mocks__/services/api"));

const TestComponent = ({ queryParams, pageSize, pageNumber, onHookReady }) => {
  // Call the hook inside a component.
  const hook = useFilms(queryParams, pageSize, pageNumber);

  // Allow tests to access the hook object.
  React.useEffect(() => {
    if (onHookReady) {
      onHookReady(hook);
    }
  }, [hook, onHookReady]);

  return (
    <div>
      {/* Render parts of the hook state to allow testing */}
      <div data-testid="films">{JSON.stringify(hook.films)}</div>
      <div data-testid="pageNumber">{hook.pageNumber}</div>
      <div data-testid="pageSize">{hook.pageSize}</div>
      <div data-testid="maxPageNumber">{hook.maxPageNumber}</div>
    </div>
  );
};

describe('useFilms hook', () => {
  beforeEach(() => {
    // Clear previous calls and set the default resolved value.
    searchFilms.mockClear();
    searchFilms.mockResolvedValue(searchFilmsResponse);
  });

  test('should fetch films on mount and transform them correctly', async () => {
    const queryParams = new URLSearchParams(
      'title=Фоллаут&year=2024&genre=DRAMA&age_restriction=SIXTEEN_PLUS'
    );

    await act(async () => {
      render(
        <TestComponent queryParams={queryParams} pageSize={50} pageNumber={0} />
      );
    });
  
    await waitFor(() => {
      const filmsEl = screen.getByTestId('films');
      const films = JSON.parse(filmsEl.textContent);
      expect(films.length).toBeGreaterThan(0);
    });
  
    screen.debug();
  
    expect(searchFilms).toHaveBeenCalledWith(
      50,
      0,
      '2024',
      'DRAMA',
      'Фоллаут',
      'SIXTEEN_PLUS'
    );
  
    const expectedFilm = {
      id: "3bfd3bca-2cd3-4c8b-99ce-c56d69319c6a",
      year: 2024,
      title: "Фоллаут",
      quality: "P1080",
      genre: "Drama",
      category: "Series",
      description:
        "Постапокалиптический мир после ядерной катастрофы. Герои борются за выживание в пустошах, где радиация, мутанты и жажда власти становятся главными угрозами.",
      thumbnail: null,
      publisher: "Amazon Prime Video",
      age_restriction: mappingJSON().age_restriction["SIXTEEN_PLUS"],
      cast_members: [
        {
          id: "d0445d71-4c2e-4815-83ba-44a1bb3e780d",
          employee_full_name: "Уолтон Гоггинс",
          role_name: "Мутант-одиночка"
        },
        {
          id: "d97f0fc7-a8d8-4417-8837-1440b1aa3802",
          employee_full_name: "Кайл МакЛахлен",
          role_name: "Ученый из убежища"
        },
        {
          id: "ada48311-18fe-410a-bd56-115f28f452cb",
          employee_full_name: "Ксения Раппопорт",
          role_name: "Лидер сопротивления"
        },
        {
          id: "14449075-86df-4976-b3ea-77b2d89b69f6",
          employee_full_name: "Элла Пернелл",
          role_name: "Выжившая"
        }
      ]
    };
  
    await waitFor(() => {
      const filmsEl = screen.getByTestId('films');
      const films = JSON.parse(filmsEl.textContent);
      expect(films).toEqual([expectedFilm]);
    });
  
    expect(screen.getByTestId('maxPageNumber')).toHaveTextContent('0');
  });

  test('should update pageNumber if it is higher than allowed maximum', async () => {
    const queryParams = new URLSearchParams();
    await act(async () => {
      render(
        <TestComponent queryParams={queryParams} pageSize={50} pageNumber={5} />
      );
    });
    await waitFor(() => {
      const pageNumberEl = screen.getByTestId('pageNumber');
      expect(pageNumberEl).toHaveTextContent('0');
    });

    const maxPageEl = screen.getByTestId('maxPageNumber');
    expect(maxPageEl).toHaveTextContent('0');
  });

  test('should refetch films when pageSize is updated', async () => {
    const queryParams = new URLSearchParams();
    let hookRef;
    const onHookReady = (hook) => {
      hookRef = hook;
    };

    const TestComponentWithRef = ({ queryParams, pageSize, pageNumber, onHookReady }) => {
      const hook = useFilms(queryParams, pageSize, pageNumber);
      React.useEffect(() => {
        if (onHookReady) onHookReady(hook);
      }, [hook, onHookReady]);
      return (
        <div>
          <div data-testid="films">{JSON.stringify(hook.films)}</div>
          <div data-testid="pageSize">{hook.pageSize}</div>
        </div>
      );
    };

    render(
      <TestComponentWithRef
        queryParams={queryParams}
        pageSize={50}
        pageNumber={0}
        onHookReady={onHookReady}
      />
    );
  
    await waitFor(() => {
      expect(screen.getByTestId('pageSize')).toHaveTextContent('50');
    });
  
    await act(async () => {
      hookRef.setPageSize(100);
    });
  
    await waitFor(() => {
      expect(screen.getByTestId('pageSize')).toHaveTextContent('100');
    });
  
    expect(searchFilms).toHaveBeenCalledWith(100, 0, null, null, null, null);
  });

  test('should allow manual refetch of films via refetchFilms', async () => {
    let hookRef;
    const onHookReady = (hook) => {
      hookRef = hook;
    };

    render(
      <TestComponent
        queryParams={new URLSearchParams()}
        pageSize={50}
        pageNumber={0}
        onHookReady={onHookReady}
      />
    );

    await screen.findByTestId('films');

    await act(async () => {
      await hookRef.refetchFilms();
    });

    expect(searchFilms).toHaveBeenCalledTimes(2);
  });

  test('should set maxPageNumber to 0 when totalPages is 0', async () => {
    const queryParams = new URLSearchParams();
  
    const emptyResponse = {
      content: [],
      totalPages: 0,
      last: true,
      totalElements: 0,
      size: 50,
      number: 0,
      sort: { sorted: false, empty: true, unsorted: true },
      first: true,
      numberOfElements: 0,
      empty: true,
    };
  
    searchFilms.mockClear();
    searchFilms.mockResolvedValue(emptyResponse);
  
    await act(async () => {
      render(<TestComponent queryParams={queryParams} pageSize={50} pageNumber={0} />);
    });
  
    await waitFor(() => {
      expect(screen.getByTestId('maxPageNumber')).toHaveTextContent('0');
    });
  
    const filmsEl = screen.getByTestId('films');
    expect(JSON.parse(filmsEl.textContent)).toEqual([]);
  });
});
