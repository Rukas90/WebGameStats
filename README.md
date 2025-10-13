# Web Quiz Game Backend

This repository contains backend services for a testing web quiz game project.

## Projects

- **Core** - Shared class library, used by both api projects.
- **IdentityApi** – Handles user authentication, email confirmation, and token management.
- **GameApi** – Manages user profiles and other game-related logic.
- **Testing** - Testing project, which includes unit and integration tests for designated services.

## Tech Stack

- ASP.NET Core
- FastEndpoints
- JwtBearer
- HCaptcha
- Azure App Services
- PostgreSQL
- Serilog
- Flurl
- FluentValidation
- MailKit
- FusionCache
- ...

## Deployment

Each API is deployed separately to its own Azure Web App and exposed via versioned REST endpoints.

## Status

Work in progress
