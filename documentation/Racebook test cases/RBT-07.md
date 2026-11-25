# RBT\-07

### Pagination - Filter applied while on a later page - Selected page changes to the last valid page

**Description:** User applies search filters while viewing a later page, to confirm the selected page changes rather than leaving an empty grid

**Preconditions:** At least 48 mods exist, and at least 16 of them belong to the same category. User is on mods page

**Flow of events:** Reducing the result set while on a page beyond the new total (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Select the category that has 16 mods | Results are filtered, the grid shows matching mods, and the active page is 1 |
| 2 | Observe the page count | Total pages reflects the filtered result count, not the unfiltered one |
| 3 | Set the category back to ALL TYPES | The full list returns |
| 4 | Navigate to page 3 again and type a search term matching no mods | An empty grid is shown with no error |
| 5 | Clear the search term | Results return and the grid is populated |
