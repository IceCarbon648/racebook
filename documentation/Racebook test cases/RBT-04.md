# RBT\-04

### Mod filters - Category selected - Only mods in that category displayed

**Description:** User filters by category to confirm that the filter applies correctly, including the reset to ALL TYPES.

**Preconditions:** Mods exist in at least two different categories. User is on Mods Page

**Flow of events:** Filtering the catalogue by category (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Open the category dropdown | A request to the categories endpoint returns 200 |
| 2 | Open the category dropdown | Options are “ALL TYPES”, “VEHICLE”, “UI”, “Pack”, “MISC”, “ENVIRONMENT” |
| 3 | Select a category that has mods | Only mods in that category are displayed |
| 4 | Select ALL TYPES | All mods are displayed again |
| 5 | Select a category with no mods | An empty grid is shown with no error |
