import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import HomePage from './HomePage';
import { renderWithProviders } from '../test/renderWithProviders';
import type { WorldDto } from '../types/world';

// Prevent real API calls
vi.mock('../api/worldsApi', () => ({ getWorlds: vi.fn() }));
vi.mock('../components', () => ({ NewWorldDialog: () => null }));

// Control mobile breakpoint without relying on vi.spyOn on a non-configurable export
let mockMediaQueryResult = false;
vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return { ...actual, useMediaQuery: () => mockMediaQueryResult };
});

import { getWorlds } from '../api/worldsApi';

const mockWorld: WorldDto = {
  id: 'world-1',
  name: 'A Very Long World Name That Could Potentially Overflow Its Container',
  author: 'A Very Verbose Author Name That Also Might Overflow',
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-06-15T12:00:00Z',
};

describe('HomePage — world cards on desktop (sm+)', () => {
  beforeEach(() => {
    mockMediaQueryResult = false;
    vi.mocked(getWorlds).mockResolvedValue([mockWorld]);
  });
  afterEach(() => vi.clearAllMocks());

  it('renders the world name', async () => {
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByText(mockWorld.name)).toBeInTheDocument());
  });

  it('renders the author name', async () => {
    renderWithProviders(<HomePage />);
    await waitFor(() =>
      expect(screen.getByText(`by ${mockWorld.author}`)).toBeInTheDocument(),
    );
  });

  it('shows the "Last updated" label on desktop', async () => {
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByText(/last updated/i)).toBeInTheDocument());
  });
});

describe('HomePage — world cards on mobile (xs)', () => {
  beforeEach(() => {
    mockMediaQueryResult = true;
    vi.mocked(getWorlds).mockResolvedValue([mockWorld]);
  });
  afterEach(() => vi.clearAllMocks());

  it('still renders the world name on mobile', async () => {
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByText(mockWorld.name)).toBeInTheDocument());
  });

  it('the "Last updated" label is present in the DOM (hidden via CSS, not removed)', async () => {
    // MUI sx display:{xs:'none',sm:'block'} suppresses the block via CSS
    // rather than conditional rendering — the element remains in the DOM.
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByText(mockWorld.name)).toBeInTheDocument());
    expect(screen.getByText(/last updated/i)).toBeInTheDocument();
  });
});

describe('HomePage — empty state', () => {
  beforeEach(() => {
    mockMediaQueryResult = false;
  });
  afterEach(() => vi.clearAllMocks());

  it('shows the empty state message when there are no worlds', async () => {
    vi.mocked(getWorlds).mockResolvedValue([]);
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByText(/no worlds yet/i)).toBeInTheDocument());
  });

  it('shows an error alert when the API call fails', async () => {
    vi.mocked(getWorlds).mockRejectedValue(new Error('network error'));
    renderWithProviders(<HomePage />);
    await waitFor(() => expect(screen.getByRole('alert')).toBeInTheDocument());
    expect(screen.getByText(/failed to load worlds/i)).toBeInTheDocument();
  });
});
