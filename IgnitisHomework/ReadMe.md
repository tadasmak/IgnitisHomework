# Power Plant Management API

This API provides Create, Read, Delete functionality for managing power plant records.

## API Reference

### 1. Retrieve Power Plants (`GET /powerplants`)

Returns a paginated list of power plants records.

**Query Parameters:**

* **`owner`**: Filters results by owner name. Supports accent-insensitive searching (e.g., 'petraite' matches 'Ona Petraitė'). See **PostgreSQL Requirement** below.

* **`pageSize`**: Sets the number of results per page. **Default: 10**.

* **`pageNumber`**: Specifies the page of results. **Default: 0**.

**Example:**

GET /powerplants?owner=petraite&pageSize=20&pageNumber=1

### 2. Add New Power Plant (`POST /powerplants`)

Creates a new power plant record using a JSON body.

**Required Fields and Constraints:**

* **`owner`**: Must be exactly **two words**, containing only letters that can be accented.

* **`power`**: Capacity must be a number between **0.0 and 200.0** (inclusive).

* **`validFrom`**: Start date for validity (`YYYY-MM-DD` format).

**Optional Field:**

* **`validTo`**: End date for validity (`YYYY-MM-DD` format).

**Example Body:**

{ "owner": "Ona Petraitė", "power": 12.5, "validFrom": "2019-09-10" }


**Success Status:** `201 Created`.

**Error Handling**: A `400 Bad Request` response will include a **detailed JSON error description** explaining which validation rules failed.

### 3. Delete Power Plant Record (`DELETE /powerplants/:id`)

Removes a record using its unique identifier.

* The record ID (integer) must be provided in the URL path.

* **Example:** `DELETE /powerplants/42`

**Success Status:** `204 No Content`.

## PostgreSQL Requirement

For the `owner` filtering in the GET endpoint to correctly handle accent-insensitive searching, your PostgreSQL database instance must have the appropriate ACCENT collation configured.

## Unit Tests

The solution includes a dedicated Test Project with unit tests implemented for the POST /powerplants endpoint, covering all validation and boundary conditions as required.

To run tests locally:

```
dotnet test