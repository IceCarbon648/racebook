# Racebook User Stories

# Registration

|  |  |
| --- | --- |
| User Story | **As a** mod creator, **I want** to register, **so that** I can upload mods that I create |
| Assumptions & Supporting Notes | - It is assumed that the user has an email address to register with |
| Business Rules | -  The email address used to create the account has to be a valid email address - Registration requires an email address, a username, and a password |
| Existing/New |  New |
| Out of Scope |  N/A |

 

#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN the registration pageWHEN the user enters a valid email address with a matching passwordTHEN the account should be created |
| 2 |  GIVEN the registration pageWHEN the user enters an invalid email addressTHEN the user should be informed accordinglyAND no account should be created |


# Logging In

|  |  |
| --- | --- |
| User Story | **As a** mod creator, **I want** to log in, **so that** I can manage (change downloadable file, delete) my uploaded mods and upload new mods |
| Assumptions & Supporting Notes | - It is assumed there is a database in which user login information will be stored |
| Business Rules | -  The email address used to create the account has to be a valid email address |
| Existing/New |  New |
| Out of Scope |  N/A |

 

#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN the login pageWHEN the user enters valid credentialsTHEN the user should be logged in |
| 2 |  GIVEN the login pageWHEN the user enters invalid credentialsTHEN the user should be informed accordinglyAND the user should be prompted to try again |


# Uploading Mods

|  |  |
| --- | --- |
| User Story | **As a** mod creator, **I want** to log in, **so that** I can manage (edit, delete) my uploaded mods and upload new mods |
| Assumptions & Supporting Notes | - It is assumed that the user has a valid account - It is assumed that the user is logged in to their account |
| Business Rules | - Each mod must have a preview image. Acceptable types are .jpg/.jpeg and .png - Acceptable file types for mods are .tpf files |
| Existing/New |  New |
| Out of Scope |  N/A |


 

#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN the mod upload pageWHEN the user has filled out relevant, required information, and clicks the “Upload” buttonTHEN the mod is successfully uploadedAND is visible to other users |
| 2 |  GIVEN the mod upload pageWHEN the user has not filled out all the required information (Title, Description, Type) and files (mod file and preview image)THEN the user should not be allowed to submit their modAND should be prompted to fill out the missing fields or files |


# Browsing Mods

|  |  |
| --- | --- |
| User Story | **As a** user, **I want** to browse available mods created by other users, **so that** I can see what is available |
| Assumptions & Supporting Notes | - It is assumed that mods will be displayed as cards on the search page |
| Business Rules | - Users should be able to search and filter mods (by type) |
| Existing/New |  New |
| Out of Scope |  N/A |


#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN the mods pageWHEN the user scrolls down the pageTHEN the user will be shown all the available mods together with key information, uploaded by other users |
| 2 | GIVEN the mods pageWHEN the user clicks on the card for a modTHEN the user will be redirected to a page listing the complete information for that mod |
| 3 | GIVEN the mods pageWHEN the user enters keywords into the search barTHEN the search results will be results containing those keywords |



# Connecting Existing Amax Account

|  |  |
| --- | --- |
| User Story | **As an** Amax account holder, **I want** to connect my Amax account, **so that** I have access to extra functionality |
| Assumptions & Supporting Notes | - It is assumed that the user has an existing Amax account |
| Business Rules | - The user should only have to connect their account once - The user should have the option to disconnect their Amax account at any time if they’re already connected |
| Existing/New |  New |
| Out of Scope |  Amax functionality is currently out of scope |


#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN user account pageWHEN the user clicks “Connect with Discord”THEN the user should be redirected to the Discord oauth2 page |


# Editing Mods

|  |  |
| --- | --- |
| User Story | **As a** mod creator, **I want** to edit mods I’ve already uploaded, **so that** other users can have access to the latest version and information |
| Assumptions & Supporting Notes | - It is assumed that the file changes delete the previously use file |
| Business Rules | - The user should only be allowed to edit the mods that they have uploaded - The user should be allowed to edit any aspect of the mod |
| Existing/New |  New |
| Out of Scope |  N/A |


#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN mod edit pageWHEN the user clicks the “save changes” buttonTHEN the changes made to the mod should be saved |



# Deleting Mods

|  |  |
| --- | --- |
| User Story | **As a** mod creator, **I want** to delete mods I’ve already uploaded, **so that** it no longer exists on the racebook platform for other users to access |
| Assumptions & Supporting Notes | - It is assumed that this action cannot be undone |
| Business Rules | - The user should only be allowed to delete the mods that they have uploaded |
| Existing/New |  New |
| Out of Scope |  N/A |


#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN my mods pageWHEN the user clicks the “delete” buttonTHEN the user should be prompted to confirm this action |


# Stats Tracking

|  |  |
| --- | --- |
| User Story | **As a** user who has connected their Amax account, **I want** to log in, **so that** I can keep track of my stats for a certain time period |
| Assumptions & Supporting Notes | - It is assumed that the user is logged in - It is assumed that the user has connected their Amax account |
| Business Rules | - The user should be able to start/stop the tracking session at any time - The user should have access to past sessions (Read-only) - A new session cannot be starting while there is an active session |
| Existing/New |  New |
| Out of Scope |  Amax functionality is currently out of scope |


 

#### Acceptance Criteria

|  |  |
| --- | --- |
| Ref | Acceptance Criteria |
| 1 | GIVEN the stats pageWHEN the user starts a new tracking session THEN the system will start tracking their stat changes from that pointAND the user should now have an option to stop the tracking session |
| 2 | GIVEN the stats pageWHEN the user stops the tracking sessionTHEN the system will stop tracking stat changes in the sessionAND the user should have the option to start a new tracking session |
| 3 | GIVEN the stats pageWHEN the user first arrives at this pageTHEN the user should be shown their current stats |
| 4 | GIVEN the stats pageWHEN the user selects a past (already ended/stopped) sessionTHEN the user will be shown their stat changes for that session |
