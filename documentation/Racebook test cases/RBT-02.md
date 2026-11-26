# RBT\-02

### Mod detail - Mod card clicked - Mod detail page displayed

**Description:** User clicks a mod card to confirm the detail view receives and renders information for the selected mod

**Preconditions:** At least one mod exists. User is on Mods page

**Flow of events:** Navigating from the mods list to a single mod (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Note the title, category and creator of the first card | Made note of values for comparison |
| 2 | Click the first mod card | Browser navigates to /mods/{modId} |
| 3 | Compare the detail page against the recorded values | Title, category and creator match the card exactly |
| 4 | Observe the description and preview image | Description and image are displayed |
| 5 | Click the download button | A .tpf file starts to download |
| 6 | Click the browser back button | Returns to the mods page with the filters intact |
