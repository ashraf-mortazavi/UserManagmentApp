# ManageUsers API Documentation

## Overview

User Management System API built with .NET 9.0, featuring **Two-Factor Authentication (2FA)** via email OTP, **role-based authorization**, **JWT authentication**, and **Problem Details** error handling.

---

## Base URL

| Environment | URL |
|---|---|
| Local Development | `https://localhost:7245` or `http://localhost:5062` |
| Docker | `http://localhost:8080` |
| Staging (via Nginx) | `https://yourdomain.com` |

## Authentication

All authenticated endpoints require a **Bearer Token** in the `Authorization` header:

```
Authorization: Bearer <jwt_token>
```

### 2FA Login Flow

```
POST /api/users/login           → Returns UserId + RequiresOtp=true + MaskedEmail
POST /api/users/verify-otp      → Returns JWT Token + RefreshToken
POST /api/users/resend-otp      → Resends OTP to user's email
```

---

## API Endpoints

### 1. Login (Step 1 of 2FA)

**`POST /api/users/login`**

Authenticates user credentials and initiates 2FA by sending an OTP to the user's email.

**Request:**
```json
{
  "userName": "Ashraf",
  "password": "Am@700511"
}
```

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "userId": "1",
      "requiresOtp": true,
      "maskedEmail": "a*****f@example.com"
    }
  }
}
```

**Error Responses:**
```json
// 401 Unauthorized - Invalid credentials
{
  "type": "https://httpstatuses.com/401",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid username or password."
}

// 404 Not Found - User doesn't exist
{
  "type": "https://httpstatuses.com/404",
  "title": "Not Found",
  "status": 404,
  "detail": "User 'invaliduser' not found."
}

// 403 Forbidden - Account disabled
{
  "type": "https://httpstatuses.com/403",
  "title": "Forbidden",
  "status": 403,
  "detail": "This account has been disabled."
}
```
---

### 4. Create User (SuperAdmin Only)

**`POST /api/users/createuser`** 🔒 *Requires role: SuperAdmin*

Creates a new user account with role assignments.

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "09123456789",
  "nationalCode": "1234567890",
  "email": "john@example.com",
  "postalCode": "1234567890",
  "userName": "johndoe",
  "password": "Str0ng@Pass",
  "personalCode": "P001",
  "position": "Developer",
  "description": "New team member",
  "organizationId": 1,
  "areaId": null,
  "regionId": null,
  "userRoleIds": ["2", "3"]
}
```

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "id": "2"
    }
  }
}
```

**Error Responses:**
```json
// 400 Bad Request - Role not found
{
  "statusCode": 400,
  "isSuccess": false,
  "errorMessage": ["نقش مورد نظر یافت نشد!"]
}

// 403 Forbidden - Not SuperAdmin
{
  "type": "https://httpstatuses.com/403",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have sufficient privileges to perform this action."
}
```

---

### 5. Change Password

**`PUT /api/users/changepassword`** 🔒 *Requires authentication*

Changes the current user's password and returns new JWT tokens.

**Request:**
```json
{
  "currentPassword": "Old@Pass1",
  "newPassword": "New@Pass1",
  "confirmNewPassword": "New@Pass1"
}
```

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "token": "eyJhbGciOiJIUzI1NiIs...",
      "refereshToken": "new-refresh-token"
    }
  }
}
```

**Error Responses:**
```json
// 400 Bad Request - Wrong current password
{
  "statusCode": 400,
  "isSuccess": false,
  "errorMessage": ["رمز عبور اشتباه است!"]
}

// 401 Unauthorized - Not authenticated
{
  "type": "https://httpstatuses.com/401",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication is required."
}
```

---

### 6. Forgot Password

**`PUT /api/users/forgotpassword`**

Sends a password reset link to the user's email.

**Request:**
```json
{
  "email": "john@example.com"
}
```

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true
}
```

---

### 7. Reset Password

**`PUT /api/users/resetpassword`**

Resets the user's password using the token from the email.

**Request:**
```json
{
  "email": "john@example.com",
  "newPassword": "Reset@Pass1",
  "confirmPassword": "Reset@Pass1"
}
```

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "success": true,
      "message": "رمز عبور با موفقیت تغییر یافت.",
      "token": "eyJhbGciOiJIUzI1NiIs...",
      "refreshToken": "new-refresh-token"
    }
  }
}
```

### 9. Get User Roles

**`GET /api/users/roles`** 🔒 *Requires authentication*

Returns the role hierarchy chain for the current user.

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "items": [
        {
          "id": 1,
          "name": "SuperAdmin",
          "nextRoleName": ""
        }
      ]
    },
    "pagination": {
      "pageNumber": 1,
      "pageSize": 10,
      "totalRecords": 1,
      "lastPage": 1
    }
  }
}
```

---

### 10. Get Role Permissions

**`GET /api/users/{roleId}/permissions`** 🔒 *Requires authentication*

Returns permissions for a specific role.

**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": [
      {
        "items": [
          {
            "roles": [
              { "id": 1, "name": "SuperAdmin" }
            ],
            "permissions": [
              { "id": 1, "key": "menu.all.view", "name": "View All", "isActive": true, "sortOrder": 0, "parentId": null }
            ]
          }
        ]
      }
    ]
  }
}
```
### 11. Post OTP Code
**`PUT /api/users/request-otp`**

**Request:**
```json
{
  "phonenumber": "09235874163"
}


**Success Response (200 OK):**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "result": {
    "data": {
      "success": true,
      "message": "رمز عبور با موفقیت تغییر یافت.",
      "otpcode": "eyJhbGciOiJIUzI1NiIs...",
      "OTPCodeValidTimeInMinute": "new-refresh-token"
    }
  }
}
```
### HTTP Status Code Mapping

| Scenario | Status Code | Title |
|---|---|---|
| Invalid OTP code | `400 Bad Request` | Invalid OTP |
| OTP expired | `400 Bad Request` | OTP Expired |
| Invalid username/password | `401 Unauthorized` | Unauthorized |
| JWT token invalid/expired | `401 Unauthorized` | Unauthorized |
| Insufficient privileges | `403 Forbidden` | Forbidden |
| User/account disabled | `403 Forbidden` | Forbidden |
| User not found | `404 Not Found` | Not Found |
| Resource not found | `404 Not Found` | Not Found |
| Validation error | `422 Unprocessable Entity` | Validation Error |
| Server error | `500 Internal Server Error` | Internal Server Error |

---

## Interactive API Documentation

| Tool | URL | Description |
|---|---|---|
| **Scalar API Reference** | `/scalar/v1` | Modern, interactive API portal (Development) |
| **OpenAPI JSON** | `/openapi/v1.json` | Raw OpenAPI 3.0 specification |

---

## Roles & Permissions

| Role | Description |
|---|---|
| `SuperAdmin` | Full system access, user management |
| `Admin` | User management, progress, operator |
| `Manager` | Managerial operations |
| `Operator` | Basic operator tasks |

### Permission Keys

| Key | Description |
|---|---|
| `menu.all.view` | View all menus |
| `menu.dashboard.view` | View dashboard |
| `menu.users.view` | View users list |
| `menu.users.create` | Create new users |
| `menu.reports.view` | View reports |
| `menu.create.progress` | Create progress entries |
| `menu.progress.view` | View progress |
| `menu.create.operator` | Create operator entries |
| `menu.view.operator` | View operator entries |

---
