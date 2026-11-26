import Login from "../pages/Login";
import MyMods from "../pages/MyMods";
import ModModal from "../pages/ModModal";
import URLs_Endpoints from "../fixtures/UrlAndEndpoints.json";
import UserData from "../fixtures/UserData.json";
import ModData from "../fixtures/ModData.json";

describe("mod upload functionality", () => {
  beforeEach(() => {
    cy.visit(URLs_Endpoints.login);

    cy.intercept("POST", URLs_Endpoints.loginEndpoint).as("loginRequest");

    Login.Login(UserData.email, UserData.password);

    cy.wait("@loginRequest").then((interception) => {
      expect(interception.response.statusCode).to.eq(200);
    });
  });

  it("Upload a mod, verify that it is listed, then delete it", () => {
    cy.visit(URLs_Endpoints.myMods);

    cy.intercept("POST", URLs_Endpoints.modsEndpoint).as("uploadRequest");

    MyMods.OpenUploadModal();
    ModModal.UploadMod(ModData);

    cy.wait("@uploadRequest").then((interception) => {
      expect(interception.response.statusCode).to.eq(201);
    });

    MyMods.VerifyThatModIsDisplayed(ModData.title);

    MyMods.DeleteMod(ModData.title);
    MyMods.VerifyThatModIsNotDisplayed(ModData.title);
  });
});
