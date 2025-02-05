module.exports = {
  jest: {
    configure: (jestConfig) => {
      return {
        ...jestConfig,
        resetMocks: false, 
        restoreMocks: false,
        clearMocks: false,
        transformIgnorePatterns: ["/node_modules/(?!(axios)/)"],
        moduleNameMapper: {
          '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
          '^@contexts/(.*)$': '<rootDir>/src/contexts/$1',
          '^@services/(.*)$': '<rootDir>/src/services/$1',
        },
        verbose: true,
        collectCoverageFrom: [
          'src/**/*.{js,jsx}',
          '!src/configs/**/*.{js,jsx}',
          '!src/main.jsx',
        ],
        testEnvironment: 'jsdom',
      };
    },
  },
};
