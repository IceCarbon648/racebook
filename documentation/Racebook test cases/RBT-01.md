# RBT\-01

### Mods list - Anonymous user navigates to the mods page - Mods displayed without favourite controls

**Description:** An anonymous visitor browses the public mods list to confirm the catalogue is readable without an account and that the favourite control is not offered.

**Preconditions:** At least 6 mods exist. No user is signed in. Currently on http://localhost:3000

**Flow of events:** Anonymous browsing of the public mods catalogue (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Navigate to the mods page | Mods page loads without errors |
| 2 | Observe the mod grid | Up to one page of mod cards is displayed, each showing title, category, creator and preview image |
| 3 | Observe a mod card | No favourite (heart) button is shown on the card |
