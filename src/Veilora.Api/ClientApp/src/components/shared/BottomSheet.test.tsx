import { vi, describe, it, expect } from 'vitest';
import { screen, fireEvent } from '@testing-library/react';
import BottomSheet from './BottomSheet';
import { renderWithProviders } from '../../test/renderWithProviders';

// useMediaQuery is mocked at the MUI module level so BottomSheet renders
// consistently regardless of jsdom's lack of real media-query support.
let mockMediaQueryResult = false;

vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return {
    ...actual,
    useMediaQuery: () => mockMediaQueryResult,
  };
});

function renderSheet(
  props: Partial<{ open: boolean; onClose: () => void; title: string; children: React.ReactNode }> = {},
) {
  const defaults = {
    open: true,
    onClose: vi.fn(),
    children: <p>Sheet body</p>,
  };
  return renderWithProviders(<BottomSheet {...defaults} {...props} />);
}

describe('BottomSheet', () => {
  it('renders children when open', () => {
    renderSheet({ open: true });
    expect(screen.getByText('Sheet body')).toBeVisible();
  });

  it('does not render children when closed', () => {
    renderSheet({ open: false });
    expect(screen.queryByText('Sheet body')).not.toBeInTheDocument();
  });

  it('renders the title when provided', () => {
    renderSheet({ title: 'My Title' });
    expect(screen.getByText('My Title')).toBeVisible();
  });

  it('does not render a title element when title is omitted', () => {
    renderSheet({ title: undefined });
    expect(screen.queryByRole('heading')).not.toBeInTheDocument();
  });

  it('renders the Drawer paper inside the document portal', () => {
    // MUI Drawer renders into a portal attached to document.body, outside
    // the container returned by render().  Query body directly.
    renderSheet({ open: true });
    expect(document.body.querySelector('.MuiDrawer-paper')).not.toBeNull();
  });

  it('calls onClose when the backdrop is clicked', () => {
    const onClose = vi.fn();
    renderSheet({ onClose });
    const backdrop = document.body.querySelector('.MuiBackdrop-root');
    if (backdrop) fireEvent.click(backdrop);
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
