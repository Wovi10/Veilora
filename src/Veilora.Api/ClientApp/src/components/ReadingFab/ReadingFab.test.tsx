import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { screen, fireEvent } from '@testing-library/react';
import ReadingFab from './ReadingFab';
import { renderWithProviders } from '../../test/renderWithProviders';
import type { ReadingSessionDto } from '../../types/readingSession';
import type { ReactNode } from 'react';

vi.mock('./StartSessionDialog', () => ({
  default: ({ open }: { open: boolean }) =>
    open ? <div data-testid="start-session-dialog" /> : null,
}));

let mockMediaQueryResult = false;
vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return { ...actual, useMediaQuery: () => mockMediaQueryResult };
});

const activeSession: ReadingSessionDto = {
  id: 'session-1',
  worldId: 'world-1',
  worldName: 'Arda',
  startedAt: '2024-06-15T10:00:00Z',
  noteCount: 2,
  isActive: true,
};

function makeSessionContext(session: ReadingSessionDto | null) {
  return {
    session,
    notes: [],
    loading: false,
    start: vi.fn(),
    pause: vi.fn().mockResolvedValue(undefined),
    resume: vi.fn().mockResolvedValue(undefined),
    clear: vi.fn().mockResolvedValue(session?.worldId ?? ''),
    captureNote: vi.fn().mockResolvedValue(null),
    removeNote: vi.fn().mockResolvedValue(undefined),
  };
}

vi.mock('../../context/ReadingSessionContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../../context/ReadingSessionContext')>();
  return {
    ...actual,
    useReadingSession: vi.fn(),
    ReadingSessionProvider: ({ children }: { children: ReactNode }) => <>{children}</>,
  };
});

import { useReadingSession } from '../../context/ReadingSessionContext';

describe('ReadingFab — mobile (xs)', () => {
  beforeEach(() => { mockMediaQueryResult = true; });
  afterEach(() => vi.clearAllMocks());

  it('renders a Fab button when there is no session', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(null));
    renderWithProviders(<ReadingFab />);
    expect(screen.getByRole('button', { name: /start reading/i })).toBeInTheDocument();
  });

  it('opens the StartSessionDialog when the Fab is tapped', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(null));
    renderWithProviders(<ReadingFab />);
    fireEvent.click(screen.getByRole('button', { name: /start reading/i }));
    expect(screen.getByTestId('start-session-dialog')).toBeInTheDocument();
  });

  it('renders a Fab when there is an active session', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
    renderWithProviders(<ReadingFab />);
    // The session Fab contains an icon but no text; just verify a button exists
    expect(screen.getAllByRole('button').length).toBeGreaterThan(0);
  });

  it('opens the BottomSheet when the session Fab is tapped', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
    renderWithProviders(<ReadingFab />);
    // Before the sheet is open, only the Fab button is rendered
    const buttons = screen.getAllByRole('button');
    fireEvent.click(buttons[0]);
    // The BottomSheet title is the world name
    expect(screen.getByText('Arda')).toBeInTheDocument();
  });

  it('the session Fab carries the MuiFab class (not a plain Button)', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
    renderWithProviders(<ReadingFab />);
    const buttons = screen.getAllByRole('button');
    const fab = buttons[0];
    expect(fab.className).toMatch(/MuiFab/);
  });
});

describe('ReadingFab — desktop (sm+)', () => {
  beforeEach(() => { mockMediaQueryResult = false; });
  afterEach(() => vi.clearAllMocks());

  it('renders a plain Button (not a Fab) when there is no session', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(null));
    renderWithProviders(<ReadingFab />);
    const startBtn = screen.getByRole('button', { name: /start reading/i });
    expect(startBtn.className).not.toMatch(/MuiFab/);
  });

  it('renders the session panel header with the world name', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
    renderWithProviders(<ReadingFab />);
    expect(screen.getByText('Arda')).toBeInTheDocument();
  });
});
