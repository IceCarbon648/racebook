# RBT\-10

### Upload mod - Executable renamed to .tpf submitted - Rejected by server side file inspection

**Description:** A signed-in user uploads a file whose extension has been changed to hide its true type, confirming the server validates content rather than just file name.

**Preconditions:** User is signed in. Mod Upload modal is opened

**Flow of events:** Attempted upload of a disguised executable (Chrome).

| **Step** | **Action** | **Expected Result** |
| --- | --- | --- |
| 1 | Fill out all information as normal except for the mod file | All input is accepted |
| 2 | For the mod file, select an executable file (.exe, .bat, .vbs, etc) that has had it’s extension renamed to .tpf | The file is accepted |
| 3 | Click the “Upload” button | A 400 response is received |
| 4 | Observe the modal | An error message is shown inline and the modal stays open |
| 5 | Close the modal and check the My Mods page | No new mod was created |
