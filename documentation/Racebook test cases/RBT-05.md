# RBT\-05

### Mod filters - Sort and reset applied together - Controls combine and clear correctly

**Description:** User combines search, category and sort, then resets, to confirm the three controls compose and that Reset returns every control to its default.

**Preconditions:** Mods exist with distinguishable upload dates across at least two categories. User is on Mods page

**Flow of events:** Combining and clearing the filter controls (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Change sort order to Oldest first | Mods reorder with the earliest upload date first |
| 2 | Select a category | Results filtered by category and ordered oldest first |
| 3 | Type a search term | Results are displayed that meet all 3 search criteria |
| 4 | Click the Reset button | Search bar is empty, category returns to ALL TYPES, sort returns to Newest first, and all mods are displayed |
