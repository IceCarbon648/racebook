# RBT\-08

### Upload mod - Valid input submitted - Mod created and visible in My Mods

**Description:** A signed-in user uploads a valid mod to confirm the happy path as well as the success toast message and cache refresh.

**Preconditions:** Currently logged in. Have access to a valid .tpf file and .jpg or .png image. Currently on My mods page

**Flow of events:** Successful mod upload from the My Mods page (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Click the “Upload Mod” button | The upload modal opens with empty fields and the heading Upload Mod |
| 2 | Fill out all fields | The input is accepted |
| 3 | Upload the valid files | The files are accepted and their names are displayed |
| 4 | Click the “Upload” button | The button becomes disabled, then the modal closes |
| 5 | Observe the toast message | A success (green) toast message is displayed |
| 6 | Observe the My Mods page | The newly uploaded mod is now displayed in a card and has an edit and delete button |
| 7 | Navigate to the Mods page | The newly uploaded mod is publicly displayed without an edit or delete button |
