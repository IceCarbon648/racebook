# RBT\-11

### Edit mod - Text fields changed with no new files - Details updated and files unchanged

**Description:** An owner edits only the text fields of an existing mod, confirming the files are preserved and the form pre-populates correctly as per the existing information on the mod being edited.

**Preconditions:** Signed into an account that has already uploaded mod(s). Currently on the My Mods page

**Flow of events:** Partial edit of an existing mod (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Click the edit button on a mod | The modal opens with the heading Edit Mod |
| 2 | Observe the form | Title, category and description are pre-filled with the mod's current values as well as the unique names for the uploaded files. |
| 3 | Change only the title to a new, non-empty, value | The new value is accepted |
| 4 | Click the “SAVE CHANGES” button | The modal closes and a success toast reading Changes saved is displayed |
| 5 | Observe the mod card | The new title is shown and the preview image is unchanged |
