class MyMods {
  elements = {
    btnUpload: () => cy.contains("button", "Upload Mod"),
    pnlModCards: () => cy.get('[data-test="mod-card"]'),
    lblModTitle: (title) => cy.contains(title),
    btnDeleteMod: (title) =>
      cy.contains('[data-test="mod-card"]', title).find('[aria-label="Delete mod"]'),
  };

  OpenUploadModal() {
    this.elements.btnUpload().click();
  }

  DeleteMod(title) {
    this.elements.btnDeleteMod(title).click();
  }

  VerifyThatMyModsPageIsDisplayed() {
    cy.url().should("include", "/my-mods");
  }

  VerifyThatModIsDisplayed(title) {
    this.elements.lblModTitle(title).should("be.visible");
  }

  VerifyThatModIsNotDisplayed(title) {
    cy.contains(title).should("not.exist");
  }
}

module.exports = new MyMods();
