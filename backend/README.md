# Thoughts.io API Documentation / API Dokumentáció

## English Version

### Overview

The **Thoughts.io API** provides a clean and modular backend for a platform where registered users can post, manage, and interact with short text entries called _thoughts_.  
The API is designed following RESTful principles, featuring authentication, authorization, CRUD operations, and relationship handling between users, roles, and thoughts.

The database schema includes:

- **Users** with authentication and role management.
- **Thoughts** representing short textual posts with visibility control.
- **Reactions** to allow users to interact with public thoughts.
- **Refresh tokens** for maintaining secure session management.

All requests and responses are in **JSON** format.

**Base URL:** `/api/v1`

---

## Authentication

The API uses **JWT (JSON Web Token)** authentication. After successful registration or login, the client receives both an access token and a refresh token.  
Access tokens are required for all protected endpoints and must be sent in the header as:

```
Authorization: Bearer <access_token>
```

When the access token expires, the refresh token can be used to obtain a new one.

---

### `POST /auth/signup`

Registers a new user account.

**Access:** Public  
**Request Body:**

```json
{
  "username": "Username123",
  "email": "test@email.com",
  "password": "StrongPassword123!",
  "verifyPassword": "StrongPassword123!"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

### `POST /auth/login`

Logs in a user using credentials.

**Access:** Public  
**Request Body:**

```json
{
  "email": "test@email.com",
  "password": "StrongPassword123!"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

### `POST /auth/refresh-token`

Generates a new access token based on a valid refresh token.

**Access:** Public  
**Request Body:**

```json
{
  "refreshToken": "<refreshToken>"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

## Thoughts

This module manages the main entity of the platform — the **thoughts**.  
A thought represents a short text message posted by a user. Each thought can be **public** (visible to everyone) or **private** (visible only to its creator).  
Users can create, edit, delete, and list their own thoughts, while public thoughts can be viewed by anyone.

---

### `GET /thoughts/public`

Fetches all **public** thoughts.

**Access:** Public  
**Response:**

```json
{
  "thoughts": [Thought]
}
```

---

### `GET /thoughts`

Fetches **all** thoughts (both public and private) of the authenticated user.

**Access:** Protected (Requires login)  
**Response:**

```json
{
  "thoughts": [Thought]
}
```

---

### `GET /thoughts/{id}`

Fetches a specific thought by ID.  
If the thought is private, only the author can access it.

**Access:** Protected (Requires login)  
**Response:**

```json
{
  "thought": Thought
}
```

---

### `POST /thoughts`

Creates a new thought.

**Access:** Protected (Requires login)  
**Request Body:**

```json
{
  "title": "My first thought",
  "content": "This is a test thought.",
  "isPublic": true
}
```

**Response:**

```json
{
  "thoughtId": "<Guid>"
}
```

---

### `PUT /thoughts/{id}`

Updates an existing thought.  
Only the creator (or an admin) can modify it.

**Access:** Protected (Requires login)  
**Request Body:**

```json
{
  "title": "Updated title",
  "content": "Updated text.",
  "isPublic": false
}
```

**Response:** `HTTP 200 - OK`

---

### `DELETE /thoughts/{id}`

Deletes a thought by ID.  
Only the author or an admin can perform this action.

**Access:** Protected (Requires login)  
**Response:** `HTTP 200 - OK`

---

## Reactions

The **Reactions** module allows users to interact with thoughts using predefined reaction types (like 👍, ❤️, etc.).  
Each reaction is associated with a user and a specific thought.  
Public thoughts’ reactions can be viewed by everyone, but adding a reaction requires authentication.

---

### `GET /thoughts/{id}/reactions`

Fetches all reactions for a given thought.

**Access:** Public  
**Response:**

```json
{
  "reactions": [Reaction]
}
```

---

### `POST /thoughts/{id}/reactions`

Adds a new reaction to a thought.

**Access:** Protected (Requires login)  
**Request Body:**

```json
{
  "thoughtId": "<Guid>",
  "reactionId": 1
}
```

**Response:**

```json
{
  "reactionId": "<Guid>"
}
```

---

## Refresh Tokens

Refresh tokens are securely stored in the database and linked to users.  
They ensure long-term session management without requiring users to log in again frequently.

Tokens expire after a given time (`ExpiresOnUtc`) and can be renewed using `/auth/refresh-token`.

---

## Admin Endpoints (Future Feature)

Planned admin functionalities include:

- Viewing and managing all users.
- Editing or removing any thought (moderation).
- Viewing basic system statistics (number of users, thoughts, etc.).

---

## Magyar Verzió

### Áttekintés

A **Thoughts.io API** egy modern és moduláris háttérrendszert biztosít, amely lehetővé teszi a regisztrált felhasználók számára rövid szöveges bejegyzések — _gondolatok_ — létrehozását és kezelését.  
Az API REST alapelvek mentén épül fel, támogatja az autentikációt, jogosultságkezelést, CRUD műveleteket és a felhasználók közötti interakciót.

Az adatbázis fő entitásai:

- **Felhasználók (Users)** – azonosítás, hitelesítés, szerepkörök.
- **Gondolatok (Thoughts)** – rövid szöveges bejegyzések, publikus vagy privát láthatósággal.
- **Reakciók (Reactions)** – interakciók a gondolatokkal.
- **Frissítő tokenek (RefreshTokens)** – biztonságos munkamenet-kezelés.

Minden kérés és válasz **JSON** formátumban történik.

**Alap URL:** `/api/v1`

---

## Autentikáció

Az API **JWT (JSON Web Token)** alapú autentikációt használ.  
Sikeres regisztráció vagy bejelentkezés után a kliens egy **access tokent** és egy **refresh tokent** kap.  
A védett végpontok eléréséhez az `Authorization` fejlécben kell megadni a tokent:

```
Authorization: Bearer <token>
```

A lejárt access tokenek frissíthetők érvényes refresh token segítségével.

---

### `POST /auth/signup`

Új felhasználó regisztrálása.

**Jogosultság:** Publikus  
**Request Body:**

```json
{
  "username": "Username123",
  "email": "teszt@email.com",
  "password": "ErosJelszo123!",
  "verifyPassword": "ErosJelszo123!"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

### `POST /auth/login`

Bejelentkezés hitelesítő adatokkal.

**Jogosultság:** Publikus  
**Request Body:**

```json
{
  "email": "teszt@email.com",
  "password": "ErosJelszo123!"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

### `POST /auth/refresh-token`

Új access token lekérése érvényes refresh token alapján.

**Jogosultság:** Publikus  
**Request Body:**

```json
{
  "refreshToken": "<refreshToken>"
}
```

**Response:**

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refreshToken>"
}
```

---

## Gondolatok (Thoughts)

Ez a modul kezeli az alkalmazás fő funkcióját: a gondolatokat.  
Egy gondolat egy rövid szöveges bejegyzés, amely lehet **publikus** vagy **privát**.  
A felhasználók létrehozhatják, módosíthatják, törölhetik és lekérdezhetik saját gondolataikat, míg a publikus bejegyzéseket bárki láthatja.

---

### `GET /thoughts/public`

Az összes publikus gondolat lekérése.

**Jogosultság:** Publikus  
**Response:**

```json
{
  "thoughts": [Thought]
}
```

---

### `GET /thoughts`

A bejelentkezett felhasználó összes gondolatának (publikus és privát) lekérése.

**Jogosultság:** Védett  
**Response:**

```json
{
  "thoughts": [Thought]
}
```

---

### `GET /thoughts/{id}`

Egy konkrét gondolat lekérése azonosító alapján.  
Privát gondolatot csak a szerző láthat.

**Jogosultság:** Védett  
**Response:**

```json
{
  "thought": Thought
}
```

---

### `POST /thoughts`

Új gondolat létrehozása.

**Jogosultság:** Védett  
**Request Body:**

```json
{
  "title": "Első gondolatom",
  "content": "Ez egy teszt gondolat.",
  "isPublic": true
}
```

**Response:**

```json
{
  "thoughtId": "<Guid>"
}
```

---

### `PUT /thoughts/{id}`

Gondolat szerkesztése azonosító alapján.

**Jogosultság:** Védett  
**Response:** `HTTP 200 - OK`

---

### `DELETE /thoughts/{id}`

Gondolat törlése azonosító alapján.

**Jogosultság:** Védett  
**Response:** `HTTP 200 - OK`

---

## Reakciók (Reactions)

A reakciók segítségével a felhasználók interakcióba léphetnek a gondolatokkal.  
Minden reakció egy adott gondolathoz és felhasználóhoz tartozik.  
A publikus gondolatok reakciói bárki számára láthatók, de reakciót hozzáadni csak bejelentkezve lehet.

---

### `GET /thoughts/{id}/reactions`

Egy gondolathoz tartozó reakciók lekérése.

**Jogosultság:** Publikus  
**Response:**

```json
{
  "reactions": [Reaction]
}
```

---

### `POST /thoughts/{id}/reactions`

Reakció hozzáadása egy gondolathoz.

**Jogosultság:** Védett  
**Request Body:**

```json
{
  "thoughtId": "<Guid>",
  "reactionId": 1
}
```

**Response:**

```json
{
  "reactionId": "<Guid>"
}
```

---

## Frissítő Tokenek (Refresh Tokens)

A refresh tokenek biztonságosan tárolódnak az adatbázisban, a felhasználókhoz kapcsolva.  
Segítségükkel hosszabb munkamenetek tarthatók fenn anélkül, hogy újra be kellene jelentkezni.  
A tokenek lejárati ideje az `ExpiresOnUtc` mezőben van megadva.

---

## Adminisztrációs Funkciók (Fejlesztés alatt)

A jövőben elérhető funkciók:

- Felhasználók kezelése.
- Gondolatok moderálása (szerkesztés, törlés).
- Statisztikai adatok megjelenítése (felhasználók, gondolatok száma stb.).

---
