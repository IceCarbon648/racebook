# RBT\-09

### Upload mod - Required fields omitted - Field level validation messages displayed

**Description:** A signed-in user submits an incomplete form to confirm each missing field reports its own message and nothing is sent to the API.

**Preconditions:** User is signed in. The upload modal is open and empty. Currently on My Mods page. 

**Flow of events:** Client-side validation on the upload form (Microsoft Edge)

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Click the “Upload Mod” button | The upload modal opens with empty fields and the heading Upload Mod |
| 2 | With every field empty, click UPLOAD, with the Network tab visible in dev tools | No request is sent to the API |
| 3 | Observe the form | A message is shown under each field where input is missing |
| 4 | Enter a title only and click the “Upload button” | The Title message clears, but the other 4 remain |
| 5 | Fill out all information except the preview image and the “Upload button” | Only the Preview Image error message is shown |
| 6 | Attempt to upload a mod file (.tpf) larger than 10mb | A message states the mod file must be smaller than 10MB |
