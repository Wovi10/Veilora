import { vi, describe, it, expect, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
import EditEntityDialog from './EditEntityDialog';
import { renderWithProviders } from '../../test/renderWithProviders';
import type { EntityDto } from '../../types/entity';

// Prevent real API calls from EditEntityDialog's save/delete handlers.
vi.mock('../../api/entitiesApi', () => ({
  updateEntity: vi.fn(),
  deleteEntity: vi.fn(),
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

const baseEntity: EntityDto = {
  id: 'entity-1',
  name: 'The Iron Guild',
  type: 'Group',
  worldId: 'world-1',
  description: 'A powerful mercenary guild.',
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
};

function renderDialog(overrides: { open?: boolean; entity?: EntityDto } = {}) {
  const props = {
    open: true,
    entity: baseEntity,
    onClose: vi.fn(),
    onSaved: vi.fn(),
    onDeleted: vi.fn(),
    ...overrides,
  };
  return renderWithProviders(<EditEntityDialog {...props} />);
}

describe('EditEntityDialog — fullScreen on mobile', () => {
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

describe('EditEntityDialog — open / closed state', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('renders the dialog title and entity name field when open', () => {
    renderDialog({ open: true });
    // The title reflects the entity type: "Edit Group"
    expect(screen.getByText(`Edit ${baseEntity.type}`)).toBeVisible();
    expect(screen.getByDisplayValue(baseEntity.name)).toBeInTheDocument();
  });

  it('does not render dialog content when open is false', () => {
    renderDialog({ open: false });
    // MUI Dialog unmounts children when open={false} with no keepMounted prop.
    expect(screen.queryByText(`Edit ${baseEntity.type}`)).not.toBeInTheDocument();
  });
});
