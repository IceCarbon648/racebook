class ModModal {
  elements = {
    textFieldTitle: () => cy.get("#title"),
    textFieldDescription: () => cy.get("#description"),
    ddlCategory: () => cy.get('button[aria-label="Select a category"]'),
    lstCategoryOption: (type) => cy.get(`[data-value="${type}"]`),
    filePickerModFile: () => cy.get("#modFile"),
    filePickerPreviewImage: () => cy.get("#previewImage"),
    btnSubmit: () => cy.get("form").contains("button", "UPLOAD")
  };

  SelectCategory(type) {
    this.elements.ddlCategory().click();
    this.elements.lstCategoryOption(type).click();
  }

  UploadMod(mod) {
    this.elements.textFieldTitle().type(mod.title);
    this.elements.textFieldDescription().type(mod.description);
    this.SelectCategory(mod.type);
    this.elements.filePickerModFile().selectFile(mod.modFile, { force: true });
    this.elements.filePickerPreviewImage().selectFile(mod.previewImage, { force: true });
    this.elements.btnSubmit().click();
  }
}

module.exports = new ModModal();
