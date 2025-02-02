module.exports = {
  jest: {
    configure: (jestConfig) => {
      jestConfig.transformIgnorePatterns = ["/node_modules/(?!(axios)/)"];
      jestConfig.moduleNameMapper = {
        "\\.(css|less|scss|sass)$": "identity-obj-proxy"
      };
      jestConfig.collectCoverageFrom = [
        "src/**/*.{js,jsx}",
        "!src/configs/**/*.{js,jsx}",
        "!src/main.jsx"
      ];
      return jestConfig;
    }
  }
};