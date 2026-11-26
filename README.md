# Racebook

A mod-sharing application for the 2010 arcade racing game "Blur"

## Overview

Blur has a small but active modding community, and the mods themselves are scattered across different threads and conversations on Discord. Racebook serves as a place to upload them to and find them easily later on. Upload a mod file with a preview image and relevant details, browse what other people have made, and favourite the ones you want to come back to later

## Tech stack

**Backend:** ASP.NET Core (.NET 10), Dapper & SQL Server, Flyway for migrations, Cloudinary for file storage, FluentValidation, Ardalis guard clauses, BCrypt for password hashing, JWT issued in an httpOnly cookie, Serilog. Unit tests done with NUnit and NSubstitute

**Frontend:** React with TypeScript, Vite, Tailwind CSS, TanStack Query, React Router, axios, Yup for form validation, react-toastify. Tested with Vitest and React Testing Library, with Storybook for component prototyping and Cypress for end-to-end tests

**Infrastructure:** Docker Compose for the whole stack and GitHub Actions for CI

### Prerequisites

- Docker Desktop
- A Cloudinary account (the free tier is enough)

### Run

```bash
docker compose up --build -d
```

This starts SQL Server, creates the database, runs the Flyway migrations, then starts the API and the frontend.

- Frontend — http://localhost:3000
- API — http://localhost:8080

Register an account through the UI to get started

## Credits

Fluid cursor effect from [Cursify](https://cursify.ui-layouts.com/components/fluid-cursor).
