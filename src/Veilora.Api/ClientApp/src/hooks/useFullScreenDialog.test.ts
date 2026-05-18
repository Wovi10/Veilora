import { vi, describe, it, expect, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import useFullScreenDialog from './useFullScreenDialog';

let mockMediaQueryResult = false;

vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return {
    ...actual,
    useMediaQuery: () => mockMediaQueryResult,
  };
});

describe('useFullScreenDialog', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('returns fullScreen: true on mobile viewports', () => {
    mockMediaQueryResult = true;
    const { result } = renderHook(() => useFullScreenDialog());
    expect(result.current.fullScreen).toBe(true);
  });

  it('returns fullScreen: false on desktop viewports', () => {
    mockMediaQueryResult = false;
    const { result } = renderHook(() => useFullScreenDialog());
    expect(result.current.fullScreen).toBe(false);
  });
});
