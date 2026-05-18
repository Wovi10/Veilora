import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { screen, fireEvent } from '@testing-library/react';
import ReadingPage from './ReadingPage';
import { renderWithProviders } from '../test/renderWithProviders';
import type { ReadingSessionDto } from '../types/readingSession';
import type { ReactNode } from 'react';

vi.mock('../components/ReadingWorkspace/ReadingWorkspace', () => ({
  default: () => <div data-testid="reading-workspace">Workspace</div>,
}));

let mockMediaQueryResult = false;
vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return { ...actual, useMediaQuery: () => mockMediaQueryResult };
});

const activeSession: ReadingSessionDto = {
  id: 'session-1',
  worldId: 'world-1',
  worldName: 'Middle Earth',
  startedAt: '2024-06-15T10:00:00Z',
  noteCount: 3,
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

vi.mock('../context/ReadingSessionContext', async (importOriginal) => {
  const actual = await importOriginal<typeof import('../context/ReadingSessionContext')>();
  return {
    ...actual,
    useReadingSession: vi.fn(),
    ReadingSessionProvider: ({ children }: { children: ReactNode }) => <>{children}</>,
  };
});

import { useReadingSession } from '../context/ReadingSessionContext';

describe('ReadingPage — no session', () => {
  it('shows the empty state prompt when there is no active session', () => {
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(null));
    renderWithProviders(<ReadingPage />);
    expect(screen.getByText(/no active session/i)).toBeInTheDocument();
  });
});

describe('ReadingPage — desktop layout (sm+)', () => {
  beforeEach(() => {
    mockMediaQueryResult = false;
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
  });
  afterEach(() => vi.clearAllMocks());

  it('renders the reading workspace', () => {
    renderWithProviders(<ReadingPage />);
    expect(screen.getByTestId('reading-workspace')).toBeInTheDocument();
  });

  it('does not render any tab buttons', () => {
    renderWithProviders(<ReadingPage />);
    expect(screen.queryAllByRole('tab')).toHaveLength(0);
  });
});

describe('ReadingPage — mobile layout (xs)', () => {
  beforeEach(() => {
    mockMediaQueryResult = true;
    vi.mocked(useReadingSession).mockReturnValue(makeSessionContext(activeSession));
  });
  afterEach(() => vi.clearAllMocks());

  it('renders a Notes tab and a Workspace tab', () => {
    renderWithProviders(<ReadingPage />);
    expect(screen.getByRole('tab', { name: /notes/i })).toBeInTheDocument();
    expect(screen.getByRole('tab', { name: /workspace/i })).toBeInTheDocument();
  });

  it('shows the Notes panel by default (Workspace is not yet rendered)', () => {
    renderWithProviders(<ReadingPage />);
    expect(screen.queryByTestId('reading-workspace')).not.toBeInTheDocument();
  });

  it('shows the Workspace panel after clicking the Workspace tab', () => {
    renderWithProviders(<ReadingPage />);
    fireEvent.click(screen.getByRole('tab', { name: /workspace/i }));
    expect(screen.getByTestId('reading-workspace')).toBeInTheDocument();
  });

  it('shows the session world name in the header', () => {
    renderWithProviders(<ReadingPage />);
    expect(screen.getByText('Middle Earth')).toBeInTheDocument();
  });
});
