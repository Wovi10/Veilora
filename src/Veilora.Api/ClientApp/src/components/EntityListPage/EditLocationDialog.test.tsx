import { vi, describe, it, expect, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
import EditLocationDialog from './EditLocationDialog';
import { renderWithProviders } from '../../test/renderWithProviders';
import type { LocationDto } from '../../types/location';

// Prevent real API calls from EditLocationDialog's save/delete handlers.
vi.mock('../../api/locationsApi', () => ({
  updateLocation: vi.fn(),
  deleteLocation: vi.fn(),
}));

// Control useMediaQuery at the MUI module level so useFullScreenDialog
// returns a deterministic value in jsdom (which has no real media queries).
let mockMediaQueryResult = false;

vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return {
    ...actual,
    useMediaQuery: () => mockMediaQueryResult,
  };
});

const baseLocation: LocationDto = {
  id: 'loc-1',
  name: 'The Dark Forest',
  worldId: 'world-1',
  description: 'A dense and dangerous wood.',
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
};

function renderDialog(overrides: { open?: boolean; location?: LocationDto } = {}) {
  const props = {
    open: true,
    location: baseLocation,
    onClose: vi.fn(),
    onSaved: vi.fn(),
    onDeleted: vi.fn(),
    ...overrides,
  };
  return renderWithProviders(<EditLocationDialog {...props} />);
}

describe('EditLocationDialog — fullScreen on mobile', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('applies the MuiDialog-paperFullScreen class when useMediaQuery returns true (mobile)', () => {
    mockMediaQueryResult = true;
    renderDialog();
    // MUI Dialog adds this class to the Paper element when fullScreen={true}.
    expect(document.body.querySelector('.MuiDialog-paperFullScreen')).not.toBeNull();
  });

  it('does not apply the MuiDialog-paperFullScreen class when useMediaQuery returns false (desktop)', () => {
    mockMediaQueryResult = false;
    renderDialog();
    expect(document.body.querySelector('.MuiDialog-paperFullScreen')).toBeNull();
  });
});

describe('EditLocationDialog — open / closed state', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('renders the dialog title and location name field when open', () => {
    renderDialog({ open: true });
    expect(screen.getByText('Edit Location')).toBeVisible();
    expect(screen.getByDisplayValue(baseLocation.name)).toBeInTheDocument();
  });

  it('does not render dialog content when open is false', () => {
    renderDialog({ open: false });
    // MUI Dialog keeps the DOM node but hides it; the title is simply absent
    // because MUI unmounts Dialog children when open={false} with no keepMounted.
    expect(screen.queryByText('Edit Location')).not.toBeInTheDocument();
  });
});
