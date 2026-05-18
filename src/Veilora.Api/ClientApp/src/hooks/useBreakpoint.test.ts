import { vi, describe, it, expect, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useIsMobile, useIsTablet } from './useBreakpoint';

// `useMediaQuery` is a non-configurable export in MUI's ESM bundle so
// vi.spyOn cannot redefine it.  We mock the whole module instead and
// control the return value via a mutable ref.
let mockMediaQueryResult = false;

vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return {
    ...actual,
    useMediaQuery: () => mockMediaQueryResult,
  };
});

describe('useIsMobile', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('returns true when the viewport is below the sm breakpoint', () => {
    mockMediaQueryResult = true;
    const { result } = renderHook(() => useIsMobile());
    expect(result.current).toBe(true);
  });

  it('returns false when the viewport is at or above the sm breakpoint', () => {
    mockMediaQueryResult = false;
    const { result } = renderHook(() => useIsMobile());
    expect(result.current).toBe(false);
  });
});

describe('useIsTablet', () => {
  beforeEach(() => { mockMediaQueryResult = false; });

  it('returns true when the viewport is in the sm–md range', () => {
    mockMediaQueryResult = true;
    const { result } = renderHook(() => useIsTablet());
    expect(result.current).toBe(true);
  });

  it('returns false when the viewport is outside the sm–md range', () => {
    mockMediaQueryResult = false;
    const { result } = renderHook(() => useIsTablet());
    expect(result.current).toBe(false);
  });
});
