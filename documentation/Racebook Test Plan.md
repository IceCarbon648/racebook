# Racebook Test Plan

# **Test plan for Racebook**

Project: Racebook  


#### Distribution

This document has been distributed to:

| **Name** | **Organisation - Role** |
| --- | --- |
| Vivek Rajaram | Kaleidocode Pivot (Pty) Ltd - Apprentice in Software Development |
| José Gonzalves | Kaleidocode Pivot (Pty) Ltd - Apprentice in Software Development |


1. **Document Purpose**

The purpose of this document is to record the Test Plan that has been adopted for the modules tested in the Racebook system. It outlines the approach, schedule, items, and requirements for the testing to be performed.


1. **Project Overview**

This project is about the mod sharing platform, Racebook, and its features (listed in point 4). Development ran over the course of year (2026), as such regression testing will need to be catered for accordingly.


1. **Objectives**

The primary objectives of testing under this plan are:

- To ensure that all development complies with the testing policy
- To ensure that features have been built according to the Business requirements and Functional specification


1. **Scope**  
**The Following items or features are within scope and are to be tested:**

- Registration
- Login
- Mod uploading
- Mod editing
- Mod deletion
- Mod searching, filtering and sorting


**The following items or features are out of scope and are not to be tested:**

- Special effects and animations


1. **Testing types, Techniques & Levels**

| **Testing Type** | **Test Technique** |
| --- | --- |
| Static - Review | Formal review of user stories and test cases |
| Functional Testing | Users story testing |
| Regression Testing | Use case testing |
| Automated Testing | Automation using Cypress |

| **Test Level** | **Performed By** |
| --- | --- |
| Unit Tests | Developers |
| Integration Testing | Functional Testers |
| System Testing | Functional Testers and Automation Testers |
| User Acceptance Testing (UAT) | BA |


1. **Roles and Responsibilities**

| **Name** | **Role** | **Responsibilities** |
| --- | --- | --- |
| Vivek Rajaram | Business Analyst | Requirements gathering & Keeping documentation up to date |
| Vivek Rajaram | Tester | Review requirements, create test plan, test cases, log bugs |
| José Gonzalves | Tester | Review requirements, create test execution, carry out manual testing, log bugs |
| Vivek Rajaram | Developer | Develop features according to specification |


1. **High Level Testing Schedule**

| **Task** | **Date** |
| --- | --- |
| Document Review | - |
| Test Case Drafting | - |
| Test Execution | - |
| Regression Testing | - |


1. **Testing Tools**

| **Testing Activity** | **Tool** |
| --- | --- |
| Test Creation | Confluence |
| Test Execution | Manual Testing |
| Defect Management | GitHub Projects |


1. **Test Environments and Credentials**

| **Item** | **QA Environment** |
| --- | --- |
| Desktop Browsers | Edge 158.0.4370.33 |
| URL | http://localhost:3000 |
| User Logins | Email: Test@mail.com Password: Banana\_12 |


1. **Test Entry and Exit Criteria**

Entry Criteria:

- Developer has completed coding for the testing required
- Developer testing has been completed
- Test cases have been drafted
- Test environment is ready for testing


Exit Criteria:

- All high priority/critical defects have been addressed
- All outstanding issues/defects have been logged
- All planned test cases have been executed
