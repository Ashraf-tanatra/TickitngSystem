# Authentication API contract

## Login

`POST /api/Auth/login`

```json
{
  "email": "user@example.com",
  "password": "user password"
}
```

The successful response includes the authenticated account and employee IDs,
the access token, and its UTC expiration time:

```json
{
  "accountId": "00000000-0000-0000-0000-000000000000",
  "employeeId": "00000000-0000-0000-0000-000000000000",
  "email": "user@example.com",
  "fName": "First",
  "lName": "Last",
  "profileImageUrl": null,
  "accessToken": "JWT",
  "accessTokenExpiresAtUtc": "2026-09-16T13:00:00Z"
}
```

## Authenticated requests

All API endpoints except `/` and `/api/Auth/*` require:

```http
Authorization: Bearer <accessToken>
```

The API derives the current account and employee from the validated token. A
client must never send an acting account or employee ID as a substitute for the
token identity.

## Authentication errors

- `401 Unauthorized`: token is missing, invalid, expired, or revoked.
- `403 Forbidden`: token is valid, but the user cannot access the resource.
- `429 Too Many Requests`: the request limit was reached. Honor `Retry-After`
  when that header is present.

Error responses contain a `message` property. Unexpected server errors also
contain a `traceId` that can be matched with the server logs.

## Protected files

Profile photos and ticket attachments are protected API resources. Requests for
their returned relative URLs must include the same bearer token. Ticket files
are available only to employees who can access the related ticket.
