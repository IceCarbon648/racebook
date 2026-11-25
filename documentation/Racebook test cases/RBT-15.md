# RBT\-15

### Favourites - Mod favourited and unfavourited - State syncs across mods and favourites pages

**Description:** A signed-in user toggles a favourite from both pages, confirming the heart state and the favourites list stay in step without a manual refresh.

**Preconditions:** Signed in with at least one mod not currently favourited.

**Flow of events:** Adding and removing a favourite (Microsoft Edge)

| Step | Action | Expected result |
| --- | --- | --- |
| 1 | Open the mods page and locate a mod that is not favourited | The heart on its card is shown in the unfilled state |
| 2 | Click the heart | The heart fills and a success toast reading Added to favourites is displayed |
| 3 | Navigate to Favourites | The mod appears in the list without a manual refresh |
| 4 | Click the heart on that card from the Favourites page | The card is removed from the list and a toast reading Removed from favourites is displayed |
| 5 | Return to the mods page | The same mod's heart is shown in the unfilled state |
