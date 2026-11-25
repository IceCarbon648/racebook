# RBT\-06

### Pagination - User navigates between pages - Correct subset of mods displayed

**Description:** A visitor pages through the catalogue to confirm each page shows a distinct subset and the controls disable at the boundaries.

**Preconditions:** More than 16 mods exist. User is on mods page

**Flow of events:** Paging through the mods catalogue (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Observe the mod made | Page 1 is active and the total page count is shown |
| 2 | Note the titles displayed on page 1 | Made note of titles for comparison |
| 3 | Observe the “Previous” button | It is disabled on page 1 |
| 4 | Click Next | Page 2 is active and shows titles that did not appear on page 1 |
| 5 | Navigate to the final page | The “Next” button is disabled |
| 6 | Return to page 1 | The original page 1 titles are shown again, in the same order |
