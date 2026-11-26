class Login {
  elements = {
    textFieldEmail: () => cy.get("#email"),
    textFieldPassword: () => cy.get("#password"),
    btnLogin: () => cy.contains("button", "LOGIN")
  };

  Login(email, password) {
    this.elements.textFieldEmail().type(email);
    this.elements.textFieldPassword().type(password);
    this.elements.btnLogin().click();
  }

  VerifyThatLoginPageIsDisplayed() {
    cy.url().should("include", "/login");
    this.elements.btnLogin().should("be.visible");
  }
}

module.exports = new Login();
