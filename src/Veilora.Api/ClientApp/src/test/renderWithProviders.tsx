import { render, type RenderOptions, type RenderResult } from '@testing-library/react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { MemoryRouter } from 'react-router-dom';
import type { ReactNode } from 'react';
import { EditModeProvider } from '../context/EditModeContext';

/**
 * Wraps a component in the minimum set of providers needed by most
 * mobile-responsive components: MUI ThemeProvider, MemoryRouter, and
 * EditModeProvider.  Extend when a test needs additional context.
 */
export function renderWithProviders(
  ui: ReactNode,
  options?: RenderOptions,
): RenderResult {
  const theme = createTheme();
  return render(
    <ThemeProvider theme={theme}>
      <MemoryRouter>
        <EditModeProvider>
          {ui}
        </EditModeProvider>
      </MemoryRouter>
    </ThemeProvider>,
    options,
  );
}
