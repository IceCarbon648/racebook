import Login from "../pages/Login";
import MyMods from "../pages/MyMods";
import UrlAndEndpoints from "../fixtures/UrlAndEndpoints.json";
import UserData from "../fixtures/UserData.json";

describe("authentication", () => {
  it("Log in and verify that a protected page can be reached", () => {
    cy.visit(UrlAndEndpoints.login);

    cy.intercept("POST", UrlAndEndpoints.loginEndpoint).as("loginRequest");

    Login.Login(UserData.email, UserData.password);

    cy.wait("@loginRequest").then((interception) => {
      expect(interception.response.statusCode).to.eq(200);
    });

    cy.visit(UrlAndEndpoints.myMods);
    MyMods.VerifyThatMyModsPageIsDisplayed();
  });

  it("Verify that an unauthenticated user is redirected to the login page", () => {
    cy.visit(UrlAndEndpoints.myMods);

    Login.VerifyThatLoginPageIsDisplayed();
  });
});
