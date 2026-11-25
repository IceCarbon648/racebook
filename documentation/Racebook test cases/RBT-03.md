# RBT\-03

### Mod filters - Search term entered - Only mods matching title or creator are displayed

**Description:** User searches using the search bar and results include titles or creators that match the search term(s)

**Preconditions:** At least one mod has a distinctive word in its title that appears in no creator name, and at least one creator has a distinctive name that appears in no title. Currently on the Mods page.

**Flow of events:** Filtering the catalogue by search term (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Type a word that appears in exactly one mod title | Only one mod is displayed, and it has a title that matches the search term |
| 2 | Click the reset button | Filters are cleared and all mods are displayed |
| 3 | Type a creator's username | Only mods by that creator are displayed |
| 4 | Click the reset button | Filters are cleared and all mods are displayed |
| 5 | Type a string that matches nothing | No mods are displayed |
