# Testing bug fixes

|  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- |
| **Test case** | **Test case name** | **Pass/Fail** | **Date** | **Tester** | **Comments/bug links/findings** |
| TT003 | Course manager tee sheet - golfers book - bookings show at the right time with golfer, players, status and fee (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. All bookings for a tee time are shown instead of just the most recent booking |
| TT004 | Book tee time – 3 players – spots reduce and booking appears in My Bookings (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. The number of available slots for a tee time are now shown |
| TT005 | Book tee time – more players than spots left – tee time cannot be selected (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. The reserve button becomes disabled when you aren’t able to book a tee time |
| TT011 | Role-based pages - user opens another role's page by URL - redirected to their own home page (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. Golfers are redirected to the course list when attempting to access pages that the golfer role is not allowed to access |
| TT012 | Sign up - email already registered, same or different capital letters - rejected (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. Users cannot sign up with an email that has allready been used to register an account |
| TT014 | Delete account - blocked by upcoming booking or owned course - allowed once clear, and user can't sign in again (Tee Time) | Pass |  | Vivek Rajaram | This now works as intended. The user is now shown the expected message when attempting to sign in to a deleted account |
