# RAT Auth API

Authentication and Authorization API built using ASP.NET Core 8 and Entity Framework Core.

This project implements the core authentication flow for an enterprise-style backend application, including user registration, JWT authentication, refresh-token rotation, logout, and role-based authorization.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- BCrypt
- JWT Bearer Authentication
- Swagger / OpenAPI
- DBeaver
- Environment Variables for configuration

## Features

- User registration
- Secure password hashing using BCrypt
- User login
- JWT access token generation
- JWT token validation
- JWT claims
- Refresh token generation
- Refresh token hashing
- Refresh token rotation
- Refresh token expiration and revocation
- Logout
- Protected endpoints
- Role-based authorization
- Admin authorization
- Global exception handling
- Request validation
- Entity Framework Core migrations
- Swagger API testing

## API Endpoints

| Method | Endpoint             | Description                           | Authentication |
| ------ | -------------------- | ------------------------------------- | -------------- |
| POST   | `/api/auth/register` | Register a new user                   | Not required   |
| POST   | `/api/auth/login`    | Authenticate user and generate tokens | Not required   |
| POST   | `/api/auth/refresh`  | Generate new access/refresh tokens    | Refresh token  |
| POST   | `/api/auth/logout`   | Revoke current refresh token          | Refresh token  |
| GET    | `/api/auth/profile`  | Get authenticated user's profile      | Required       |
| GET    | `/api/auth/admin`    | Admin-only endpoint                   | Admin role     |

## Authentication Flow

### Registration

```text
Client
  |
  | POST /api/auth/register
  v
Auth API
  |
  | Validate request
  | Hash password
  | Create user
  v
SQL Server
```

### Login

```text
Client
  |
  | POST /api/auth/login
  v
Auth API
  |
  | Validate credentials
  |
  |--------------------|
  |                    |
  v                    v
Access Token       Refresh Token
(short-lived)      (long-lived)
                       |
                       v
                  Hash + Store
                    in DB
```

### Refresh

```text
Refresh Token A
      |
      | POST /api/auth/refresh
      v
Validate Token A
      |
      v
Revoke Token A
      |
      v
Generate Token B
      |
      v
Return new Access + Refresh Token
```

### Logout

```text
Refresh Token
      |
      | POST /api/auth/logout
      v
Revoke Refresh Token
```

### Profile

```text
GET /api/auth/profile
Authorization: Bearer <access-token>

Requires: [Authorize]
```

### Admin

```text
GET /api/auth/admin
Authorization: Bearer <access-token>

Requires: [Authorize(Roles = "Admin")]
```

## Configuration

Sensitive configuration such as database credentials and JWT signing keys should be provided through environment variables.

- Example:

```text
ConnectionStrings__DefaultConnection=<connection-string>

JWT_KEY=<strong-secret>
JWT_ISSUER=RAT_AUTH_API
JWT_AUDIENCE=RAT_AUTH_CLIENTs
JWT_EXPIRATION_MINUTES=15
```

## Database
The API uses SQL Server with Entity Framework Core.


### Main tables:

```text
Users
  |
  | 1 : many
  |
RefreshTokens
```

- Users

Stores:

User ID
First name
Last name
Email
Password hash
Role
Created date


- RefreshTokens

Stores:

Refresh token ID
Token hash
User ID
Expiration date
Revocation date

Raw refresh tokens are not stored in the database.


