---
name: feedback_mui_dialog_fullscreen_assertion
description: How to assert MUI Dialog fullScreen state in jsdom tests — use document.body.querySelector class selector
metadata:
  type: feedback
---

When asserting that a MUI Dialog renders in fullscreen mode, query for the class `.MuiDialog-paperFullScreen` on `document.body`:

```ts
expect(document.body.querySelector('.MuiDialog-paperFullScreen')).not.toBeNull(); // fullscreen
expect(document.body.querySelector('.MuiDialog-paperFullScreen')).toBeNull();     // not fullscreen
```

MUI applies this class to the Dialog Paper element when `fullScreen={true}`. The Dialog renders into a portal on `document.body`, so `screen.getBy*` queries will not find it — query `document.body` directly instead.

**Why:** MUI Dialog portals render outside the test container. The class `MuiDialog-paperFullScreen` is the canonical marker MUI uses for the fullscreen Paper variant; it's stable across MUI v5/v6.

**How to apply:** Any test asserting responsive fullscreen dialog behaviour in this project. See also [[feedback_mui_mediaquery_mock]].
