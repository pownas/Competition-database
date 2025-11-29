# Competition Results API Documentation

This document describes the backend Web API for receiving competition results from the "Judge" page in the client project.

## Overview

The Competition Results API provides endpoints to create, update, and retrieve competition results submitted by judges. The API supports POST and PUT operations for submitting and updating results, with built-in validation and error handling.

## Base URL

```
/api/competitionresults
```

## Data Models

### CompetitionResultDto (Request Body)

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `competitionId` | integer | Yes | The competition identifier |
| `judgeId` | integer | Yes | The judge identifier submitting the results |
| `results` | array | Yes | List of contestant scores (minimum 1 item) |

### ContestantScoreDto (Result Item)

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `contestantId` | integer | Yes | The contestant's identifier |
| `name` | string | No | The contestant's name |
| `score` | double | Yes | The score given to the contestant |

### Valid Score Values

| Score | Vote Type | Description |
|-------|-----------|-------------|
| 10 | Yes | Contestant passes |
| 4.5 | Alt 1 | First alternate |
| 4.3 | Alt 2 | Second alternate |
| 4.2 | Alt 3 | Third alternate |
| 0 | No | Contestant does not pass |

### ContestantResult (Response)

| Field | Type | Description |
|-------|------|-------------|
| `id` | integer | Unique identifier for the result |
| `contestantId` | integer | The contestant's identifier |
| `name` | string | The contestant's name |
| `score` | double | The score given |
| `competitionId` | integer | The competition identifier |
| `judgeId` | integer | The judge identifier |
| `submittedAt` | datetime | Timestamp when the result was submitted |

## Endpoints

### POST /api/competitionresults

Submit new competition results from a judge.

**Request:**
```json
{
  "competitionId": 1,
  "judgeId": 1,
  "results": [
    { "contestantId": 10, "name": "Jonas", "score": 10 },
    { "contestantId": 24, "name": "Sara", "score": 4.5 },
    { "contestantId": 27, "name": "Elin", "score": 0 },
    { "contestantId": 32, "name": "Helena", "score": 4.3 }
  ]
}
```

**Responses:**

| Status Code | Description |
|-------------|-------------|
| 201 Created | Results created successfully |
| 400 Bad Request | Invalid request data |
| 409 Conflict | Results already exist for this competition/judge combination |

**Example Success Response (201):**
```json
[
  {
    "id": 1,
    "contestantId": 10,
    "name": "Jonas",
    "score": 10,
    "competitionId": 1,
    "judgeId": 1,
    "submittedAt": "2024-01-15T12:00:00Z"
  },
  {
    "id": 2,
    "contestantId": 24,
    "name": "Sara",
    "score": 4.5,
    "competitionId": 1,
    "judgeId": 1,
    "submittedAt": "2024-01-15T12:00:00Z"
  }
]
```

### PUT /api/competitionresults

Update existing competition results from a judge.

**Request:** Same format as POST

**Responses:**

| Status Code | Description |
|-------------|-------------|
| 200 OK | Results updated successfully |
| 400 Bad Request | Invalid request data |
| 404 Not Found | No existing results found to update |

### GET /api/competitionresults/competition/{competitionId}

Get all results for a specific competition (from all judges).

**Example:** `GET /api/competitionresults/competition/1`

**Response:** Array of ContestantResult objects

### GET /api/competitionresults/competition/{competitionId}/judge/{judgeId}

Get results submitted by a specific judge for a competition.

**Example:** `GET /api/competitionresults/competition/1/judge/1`

**Response:** Array of ContestantResult objects

## Client Integration Example

### From the Judge Page (Blazor WebAssembly)

```csharp
@using System.Net.Http.Json
@inject HttpClient Http

// Create the DTO with contestant scores
var dto = new CompetitionResultDto
{
    CompetitionId = 1,
    JudgeId = 1,
    Results = contestants.Select(c => new ContestantScoreDto
    {
        ContestantId = c.Id,
        Name = c.Name,
        Score = c.Scored
    }).ToList()
};

// POST for first submission
var response = await Http.PostAsJsonAsync("api/competitionresults", dto);

if (response.IsSuccessStatusCode)
{
    // Results saved successfully
}
else if (response.StatusCode == HttpStatusCode.Conflict)
{
    // Results already exist, use PUT to update
    response = await Http.PutAsJsonAsync("api/competitionresults", dto);
}
```

### From JavaScript (fetch API)

```javascript
// Submit new results
const submitResults = async (competitionId, judgeId, results) => {
  const response = await fetch('/api/competitionresults', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      competitionId: competitionId,
      judgeId: judgeId,
      results: results.map(r => ({
        contestantId: r.id,
        name: r.name,
        score: r.score
      }))
    })
  });

  if (response.ok) {
    return await response.json();
  } else if (response.status === 409) {
    // Already submitted, update instead
    return updateResults(competitionId, judgeId, results);
  }
  
  throw new Error(`Failed to submit: ${response.statusText}`);
};

// Update existing results
const updateResults = async (competitionId, judgeId, results) => {
  const response = await fetch('/api/competitionresults', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      competitionId: competitionId,
      judgeId: judgeId,
      results: results.map(r => ({
        contestantId: r.id,
        name: r.name,
        score: r.score
      }))
    })
  });

  if (response.ok) {
    return await response.json();
  }
  
  throw new Error(`Failed to update: ${response.statusText}`);
};

// Get results for a competition
const getCompetitionResults = async (competitionId) => {
  const response = await fetch(`/api/competitionresults/competition/${competitionId}`);
  return await response.json();
};
```

## Error Handling

All error responses follow the Problem Details format (RFC 7807):

```json
{
  "title": "Invalid request",
  "detail": "CompetitionId must be greater than 0",
  "status": 400
}
```

### Common Validation Errors

- `CompetitionId must be greater than 0`
- `JudgeId must be greater than 0`
- `At least one result is required`
- `ContestantId must be greater than 0`
- `Invalid score {value}. Valid scores are: 0 (No), 4.2 (Alt3), 4.3 (Alt2), 4.5 (Alt1), 10 (Yes)`

## Notes

- The current implementation uses in-memory storage. Data is lost when the server restarts.
- For production use, consider implementing a database backend (Entity Framework Core recommended).
- Results are uniquely identified by the combination of `competitionId` and `judgeId`.
- Each judge can only submit one set of results per competition. Use PUT to update existing results.
