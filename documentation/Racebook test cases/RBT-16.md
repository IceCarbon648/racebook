# RBT\-16

### Protected routes - Anonymous user requests a protected page - Redirected to login

**Description:** An anonymous user attempts to reach a page requiring authentication by manually entering the URL, confirming the route guard redirects to login.

**Preconditions:** No user is signed. 

**Flow of events:** Direct navigation to protected routes while signed out (Microsoft Edge)

| Step | Action | Expected result |
| --- | --- | --- |
| 1 | Navigate directly to the Favourites URL (http://localhost:3000/favourites) | Redirected to the login page |
| 2 | Press the browser Back button | The user is not returned to a protected page in a redirect loop |
