# RBT\-14

### Delete mod - Mod favourited by another user deleted - Favourite reference removed

**Description:** Mod owner deletes a mod that another user has favourited, confirming the database cascade removes the favourite row.

**Preconditions:** Have access to 2 account. One account is the creator of the mod and the other has favourited the same mod. Signed into first account. Currently of Mods page

**Flow of events:** Deletion of a mod referenced in another user's favourites (Microsoft Edge)

| Step | Action | Expected result |
| --- | --- | --- |
| 1 | Favourite a mod that is owned by the second account | The favourite (heart) button outline is now filled in and resembles a solid white heart |
| 2 | Navigate to the favourites page and verify that the mod that has just been favourited appears here | The recently favourited mod appears in the Favourites page |
| 3 | Sign out and sign in as owner of the mod that was favourited (second account) | Owner's session is active |
| 4 | Click Delete and confirm | The deletion succeeds |
| 6 | Observe the toast and the grid | Success toast shown and the card is removed |
| 7 | Sign out, then sign into the first account | The session of the first account is now active |
| 8 | Navigate to the Favourites page | Verify that the deleted mod does not appear in the favourites page |
