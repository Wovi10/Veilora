import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { screen, fireEvent, waitFor } from '@testing-library/react';
import { render } from '@testing-library/react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { MemoryRouter } from 'react-router-dom';
import { EditModeProvider } from './context/EditModeContext';
import { ThemeModeProvider } from './context/ThemeModeContext';
import { AuthProvider } from './context/AuthContext';
import App from './App';
import type { ReactNode } from 'react';

vi.mock('./api/readingSessionsApi', () => ({
  getCurrentSession: () => Promise.resolve(null),
  getNotes: () => Promise.resolve([]),
  startSession: vi.fn(),
  pauseSession: vi.fn(),
  resumeSession: vi.fn(),
  clearSession: vi.fn(),
  addNote: vi.fn(),
  deleteNote: vi.fn(),
}));

vi.mock('react-router-dom', async (importOriginal) => {
  const actual = await importOriginal<typeof import('react-router-dom')>();
  return { ...actual, Outlet: () => <div data-testid="outlet" /> };
});

// Mock ReadingWorkspace so the note editor doesn't pull in TipTap
vi.mock('./components/ReadingFab/StartSessionDialog', () => ({
  default: () => null,
}));

let mockMediaQueryResult = false;
vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return { ...actual, useMediaQuery: () => mockMediaQueryResult };
});

vi.mock('./context/ReadingSessionContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('./context/ReadingSessionContext')>();
  return {
    ...actual,
    useReadingSession: () => ({
      session: null,
      notes: [],
      loading: false,
      start: vi.fn(),
      pause: vi.fn(),
      resume: vi.fn(),
      clear: vi.fn(),
      captureNote: vi.fn(),
      removeNote: vi.fn(),
    }),
    ReadingSessionProvider: ({ children }: { children: ReactNode }) => <>{children}</>,
  };
});

function renderApp() {
  const theme = createTheme();
  return render(
    <AuthProvider>
      <ThemeModeProvider>
        <EditModeProvider>
          <ThemeProvider theme={theme}>
            <MemoryRouter>
              <App />
            </MemoryRouter>
          </ThemeProvider>
        </EditModeProvider>
      </ThemeModeProvider>
    </AuthProvider>,
  );
}

describe('App — AppBar on desktop (sm+)', () => {
  beforeEach(() => { mockMediaQueryResult = false; });
  afterEach(() => vi.clearAllMocks());

  it('shows the Edit toggle button in the AppBar', () => {
    renderApp();
    expect(screen.getByRole('button', { name: /edit/i })).toBeInTheDocument();
  });

  it('shows the Logout button in the AppBar', () => {
    renderApp();
    expect(screen.getByRole('button', { name: /logout/i })).toBeInTheDocument();
  });

  it('renders the Notes button as a labelled button with text', () => {
    renderApp();
    // On desktop NotesButton renders with "Notes" text
    expect(screen.getByRole('button', { name: /notes/i })).toBeInTheDocument();
  });
});

describe('App — AppBar on mobile (xs)', () => {
  beforeEach(() => { mockMediaQueryResult = true; });
  afterEach(() => vi.clearAllMocks());

  it('renders the AppBar landmark on mobile', () => {
    renderApp();
    expect(screen.getByRole('banner')).toBeInTheDocument();
  });

  it('renders the Notes icon button on mobile (icon-only, no "Notes" text label)', () => {
    renderApp();
    // On mobile the NotesButton renders as an IconButton, not a labelled Button.
    // An IconButton has role="button" but no text content matching "Notes".
    const notesBtns = screen.queryAllByRole('button', { name: /notes/i });
    expect(notesBtns).toHaveLength(0);
  });
});

describe('App — DrawerContent on mobile', () => {
  beforeEach(() => { mockMediaQueryResult = true; });
  afterEach(() => vi.clearAllMocks());

  /** Returns the drawer Paper element (rendered in a portal on document.body). */
  async function openDrawerAndGetPaper(): Promise<Element> {
    renderApp();
    const menuButton = screen.getAllByRole('button')[0];
    fireEvent.click(menuButton);
    await waitFor(() => expect(screen.getByRole('heading', { name: /veilora/i })).toBeInTheDocument());
    const paper = document.querySelector('.MuiDrawer-paper');
    if (!paper) throw new Error('Drawer paper not found');
    return paper;
  }

  it('shows Edit Mode list item inside the drawer when mobile', async () => {
    const paper = await openDrawerAndGetPaper();
    expect(paper.textContent).toMatch(/edit mode/i);
  });

  it('shows Logout list item inside the drawer when mobile', async () => {
    const paper = await openDrawerAndGetPaper();
    // The drawer item has text "Logout" — distinct from the AppBar LogoutButton
    const logoutItem = paper.querySelector('[class*="MuiListItemText"]');
    expect(paper.textContent).toMatch(/logout/i);
    expect(logoutItem).not.toBeNull();
  });

  it('does not show Edit Mode or Logout list items in the drawer on desktop', async () => {
    mockMediaQueryResult = false;
    renderApp();
    const menuButton = screen.getAllByRole('button')[0];
    fireEvent.click(menuButton);
    await waitFor(() => expect(screen.getByRole('heading', { name: /veilora/i })).toBeInTheDocument());
    const paper = document.querySelector('.MuiDrawer-paper');
    if (!paper) throw new Error('Drawer paper not found');
    // On desktop, DrawerContent does not render the isMobile block so the
    // drawer should not contain "Edit Mode" text or a dedicated Logout item.
    expect(paper.textContent).not.toMatch(/edit mode/i);
    // "Logout" is absent from the drawer on desktop (it lives in the AppBar)
    expect(paper.textContent).not.toMatch(/logout/i);
  });
});
