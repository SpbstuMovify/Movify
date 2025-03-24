import { TEST_FILM_DATA, TEST_PREVIEW_IMAGE_HEIGHT, TEST_PREVIEW_IMAGE_WIDTH } from "../fixtures/e2e_test_config";

describe("Adding to favorites", () => {
    beforeEach(() => {
        cy.clearTestFilms();
        cy.deleteTestUser();

        cy.createTestFilm();
        cy.createTestUser();
        cy.logInTestUser();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("adds required film to favorites", () => {
        cy.contains('Add to favorites').click();
        cy.contains('Remove from favorites').should("be.visible");

        cy.get("a").contains("Favorites").click();
        cy.get('h2[data-test-cy="film-title"]').contains(TEST_FILM_DATA.title).should("be.visible");
    })
})

describe("Removing from favorites", () => {
    beforeEach(() => {
        cy.deleteTestUser();
        cy.clearTestFilms();

        cy.createTestFilm();

        cy.createTestUser();
        cy.logInTestUser();

        cy.addTestFilmToFavorites();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("removes required film from favorites", () => {
        cy.contains('Remove from favorites').click();
        cy.contains('Add to favorites').should("be.visible");

        cy.get("a").contains("Favorites").click();
        cy.contains("No films found...").should("be.visible");
    })
})

describe("Editing film info", () => {
    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();
        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("saves the edited info successfully", () => {
        cy.get('div[data-test-cy="editable-field"]').contains('div', TEST_FILM_DATA.title)
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type("TEST_TITLE");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Genre')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('select').select("DRAMA");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Category')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('select').select("ANIMATED_SERIES");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Age restriction')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('select').select("SIX_PLUS");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Publisher')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type("TEST_PUBLISHER");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Release year')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type(2000);
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Description')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('textarea').clear().type("DESCRIPTION");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('button[data-test-cy="add-cast-member-button"]').click();

        for(let i = 0; i < 4; ++i)
        {
            cy.get('button[data-test-cy="remove-cast-member-button"]').first().click();
        }

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Actor')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type("TEST_ACTOR");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'Role')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type("TEST_ROLE");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.reload();

        cy.contains("TEST_TITLE").should("be.visible");
        cy.contains("Drama").should("be.visible");
        cy.contains("Animated series").should("be.visible");
        cy.contains("6+").should("be.visible");
        cy.contains("TEST PUBLISHER").should("be.visible");
        cy.contains("2000").should("be.visible");
        cy.contains("DESCRIPTION").should("be.visible");
        cy.contains("TEST_ACTOR").should("be.visible");
        cy.contains("TEST_ROLE").should("be.visible");
        cy.contains("Райан Гослинг").should("not.exist");
        cy.contains("Кен").should("not.exist");
    })
})

describe("Uploading the film preview", () => {
    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("chages preview successfully", () => {
        cy.get('input[data-testid="image-upload"]')
        .selectFile("./cypress/fixtures/test_preview.png", {force: true});
        cy.wait(3000);
        cy.get('img[alt="Updatable"]').should('be.visible').and(($img) => {
            expect($img[0].naturalWidth).equal(257);
            expect($img[0].naturalHeight).equal(385);
        })

        cy.reload();

        cy.get('img[alt="Updatable"]').should('be.visible').and(($img) => {
            expect($img[0].naturalWidth).equal(TEST_PREVIEW_IMAGE_WIDTH);
            expect($img[0].naturalHeight).equal(TEST_PREVIEW_IMAGE_HEIGHT);
        })
    })
})

describe("Creating a new episode and season", () => {
    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("creates a new episode and season successfully", () => {
        cy.contains(/Add Episode/i).click();
        cy.get('input[data-test-cy="add-season-input"]').type("5");
        cy.get('input[data-test-cy="add-episode-input"]').type("5");
        cy.contains(/Add Episode/i).click();

        cy.get('select[data-test-cy="season-select"]').select("5");
        cy.get('select[data-test-cy="episode-select"]').select("5");

        cy.get('div[data-test-cy="player-placeholder"]').should("be.visible");
        cy.contains('h2', /New Episode/i).should("be.visible");
    })
})

describe("Uploading an episode video", () => {
    const EPISODE_NUMBER = 5;
    const SEASON_NUMBER = 5;

    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();
        cy.createTestFilmEpisode(SEASON_NUMBER, EPISODE_NUMBER);

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("uploads an episode video successfully", () => {

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.get('div[data-test-cy="player-placeholder"]').should("be.visible");

        cy.get('input[data-testid="episode-upload"]')
        .selectFile("./cypress/fixtures/test_video.mp4", {force: true});

        cy.wait(1000)

        cy.reload();

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.get('[data-test-cy="react-player"]').should('be.visible');
    })
})

describe("Editing new episode info", () => {
    const EPISODE_NUMBER = 5;
    const SEASON_NUMBER = 5;

    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();
        cy.createTestFilmEpisode(SEASON_NUMBER, EPISODE_NUMBER);

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("edits new episode's info successfully", () => {

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.get('div[data-test-cy="player-placeholder"]').should("be.visible");
        cy.contains('h2', /New Episode/i).should("be.visible");

        cy.get('div[data-test-cy="editable-field"]').contains('div', 'New Episode')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('input').clear().type("TEST_EPISODE_TITLE");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.get('.player-wrapper').contains('div', 'Description')
        .find('button[data-testid="edit-image-button"]').click();
        cy.get('div[data-test-cy="editable-field"]').find('textarea').clear().type("EPISODE_DESCRIPTION");
        cy.get('button[data-testid="save-image-button"]').click();

        cy.reload();

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.contains('TEST_EPISODE_TITLE').should("be.visible");
        cy.contains('EPISODE_DESCRIPTION').should("be.visible");
    })
})

describe("Deleting episode", () => {
    const EPISODE_NUMBER = 5;
    const SEASON_NUMBER = 5;

    beforeEach(() => {
        cy.clearTestFilms();

        cy.createTestFilm();
        cy.createTestFilmEpisode(SEASON_NUMBER, EPISODE_NUMBER);

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();
    })
    it("deletes the episode successfully", () => {

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.get('div[data-test-cy="player-placeholder"]').should("be.visible");
        cy.contains('h2', /New Episode/i).should("be.visible");

        cy.get('button').contains(/Delete/i).click();
        
        cy.get('div[data-test-cy="player-placeholder"]').should("not.exist");

        cy.reload();

        cy.get('select[data-test-cy="season-select"]').contains("5").should("not.exist");
    })
})

describe("Watching an episode", () => {
    const EPISODE_NUMBER = 5;
    const SEASON_NUMBER = 5;

    beforeEach(() => {
        cy.clearLocalStorage();
        cy.clearCookies();
        cy.window().then((win) => {
            win.performance.clearResourceTimings();
        });

        cy.clearTestFilms();

        cy.createTestFilm();
        cy.createTestFilmEpisode(SEASON_NUMBER, EPISODE_NUMBER);

        cy.logInTestAdmin();
        
        cy.visit("/films");
        cy.get('input[placeholder="Search..."]').type(TEST_FILM_DATA.title);
        cy.get('button[data-test-cy="search-button"]').click();
        cy.get('div[data-test-cy="film-button"]').contains(TEST_FILM_DATA.title).click();

        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);

        cy.get('input[data-testid="episode-upload"]')
        .selectFile("./cypress/fixtures/test_video.mp4", {force: true});
        
        cy.wait(20000);
        cy.intercept('GET', '**/master.m3u8').as('m3u8');
        cy.intercept('GET', '**/360p.m3u8').as('m3u8_360');
        cy.intercept('GET', '**/1080p.m3u8').as('m3u8_1080');
        cy.intercept('GET', '**/1080p_*.ts').as('ts_1080');

        cy.reload();
    })
    it("loads video by chunks of different quality", () => {
        cy.get('select[data-test-cy="season-select"]').select(`${SEASON_NUMBER}`);
        cy.get('select[data-test-cy="episode-select"]').select(`${EPISODE_NUMBER}`);
        
        cy.get('video').should('have.attr', 'src');

        cy.get('video').then(($video) => {
            $video[0].play();
        });

        cy.wait('@m3u8').then((interception) => {
        expect(interception.response.statusCode).to.eq(200);
        expect(interception.request.url).to.include('master.m3u8');
        });

        cy.wait('@m3u8_360').then((interception) => {
            expect(interception.response.statusCode).to.eq(200);
            expect(interception.request.url).to.include('360p.m3u8');
        });

        cy.wait('@m3u8_1080').then((interception) => {
            expect(interception.response.statusCode).to.eq(200);
            expect(interception.request.url).to.include('1080p.m3u8');
        });

        cy.wait('@ts_1080').then((interception) => {
            expect(interception.response.statusCode).to.eq(200);
            expect(interception.request.url).to.match(/1080p_\d+\.ts$/);
            });
        })
})