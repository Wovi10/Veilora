import '@testing-library/jest-dom';

// jsdom does not implement window.matchMedia.
// ThemeModeContext calls it during useState initialisation, so we need a stub
// available before any component mounts.
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: (query: string): MediaQueryList => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => {},
    removeListener: () => {},
    addEventListener: () => {},
    removeEventListener: () => {},
    dispatchEvent: () => false,
  }),
});
