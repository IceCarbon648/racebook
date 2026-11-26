# RBT\-13

### Delete mod - Deletion confirmed - Mod and its files removed

**Description:** An owner deletes a mod, all stored information pertaining to the mod is removed as well as the card

**Preconditions:** Signed into an account that has already uploaded mod(s). Currently on the My Mods page

**Flow of events:** Confirmed deletion of an owned mod (Microsoft Edge)

| Step | Action | Expected result |
| --- | --- | --- |
| 1 | Navigate to My Mods and click Delete on a mod | A confirmation prompt is displayed |
| 2 | Confirm the prompt | The request is sent to the server and responds with a 200 http status code |
| 3 | Observe the mod grid | The mod's card is removed without a manual page refresh |
| 4 | Observe the toast message | A success toast reading Mod deleted is displayed |
| 5 | Navigate to the public mods page | The deleted mod no longer appears |
