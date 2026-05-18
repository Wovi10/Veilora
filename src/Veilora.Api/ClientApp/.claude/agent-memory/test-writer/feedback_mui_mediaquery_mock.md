---
name: feedback_mui_mediaquery_mock
description: Established pattern for mocking MUI useMediaQuery in this project's Vitest tests
metadata:
  type: feedback
---

The project's Vitest tests mock `useMediaQuery` by hoisting a module-level `let` variable and replacing the export through `vi.mock('@mui/material', ...)`:

```ts
let mockMediaQueryResult = false;

vi.mock('@mui/material', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@mui/material')>();
  return {
    ...actual,
    useMediaQuery: () => mockMediaQueryResult,
  };
});
```

Flip `mockMediaQueryResult` before each test case to simulate mobile (`true`) or desktop (`false`). Reset it in `beforeEach`.

**Why:** jsdom does not implement `window.matchMedia`, and `vi.spyOn` cannot override a non-configurable export. This factory mock pattern is the only reliable approach. All existing test files in the project (`BottomSheet.test.tsx`, `HomePage.test.tsx`, `useFullScreenDialog.test.ts`) use this exact shape.

**How to apply:** Any test that needs to exercise responsive/breakpoint-dependent behaviour. Do NOT use `vi.spyOn(mui, 'useMediaQuery')` — it does not work on this project's MUI build. See also [[feedback_mui_dialog_fullscreen_assertion]].
