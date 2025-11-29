using System.Collections.Concurrent;

namespace CompetitionDB.Services;

using CompetitionDB.Models;

/// <summary>
/// In-memory implementation of competition result storage.
/// </summary>
public class InMemoryCompetitionResultService : ICompetitionResultService
{
    private readonly ConcurrentDictionary<string, List<ContestantResult>> _storage = new();
    private int _nextId = 1;
    private readonly object _idLock = new();

    private static string GetKey(int competitionId, int judgeId) => $"{competitionId}_{judgeId}";

    /// <inheritdoc />
    public Task<List<ContestantResult>> SaveResultsAsync(CompetitionResultDto dto)
    {
        ValidateDto(dto);

        var results = dto.Results.Select(r =>
        {
            int id;
            lock (_idLock)
            {
                id = _nextId++;
            }
            return new ContestantResult
            {
                Id = id,
                ContestantId = r.ContestantId,
                Name = r.Name,
                Score = r.Score,
                CompetitionId = dto.CompetitionId,
                JudgeId = dto.JudgeId,
                SubmittedAt = DateTime.UtcNow
            };
        }).ToList();

        var key = GetKey(dto.CompetitionId, dto.JudgeId);

        if (_storage.ContainsKey(key))
        {
            throw new InvalidOperationException(
                $"Results already exist for competition {dto.CompetitionId} from judge {dto.JudgeId}. Use PUT to update.");
        }

        _storage[key] = results;
        return Task.FromResult(results);
    }

    /// <inheritdoc />
    public Task<List<ContestantResult>> UpdateResultsAsync(CompetitionResultDto dto)
    {
        ValidateDto(dto);

        var key = GetKey(dto.CompetitionId, dto.JudgeId);

        if (!_storage.TryGetValue(key, out var existingResults))
        {
            throw new KeyNotFoundException(
                $"No results found for competition {dto.CompetitionId} from judge {dto.JudgeId}. Use POST to create new results.");
        }

        // Update existing results, keeping original IDs and timestamps
        var updatedResults = dto.Results.Select(r =>
        {
            var existing = existingResults.FirstOrDefault(e => e.ContestantId == r.ContestantId);
            return new ContestantResult
            {
                Id = existing?.Id ?? GetNextId(),
                ContestantId = r.ContestantId,
                Name = r.Name,
                Score = r.Score,
                CompetitionId = dto.CompetitionId,
                JudgeId = dto.JudgeId,
                SubmittedAt = DateTime.UtcNow
            };
        }).ToList();

        _storage[key] = updatedResults;
        return Task.FromResult(updatedResults);
    }

    /// <inheritdoc />
    public Task<List<ContestantResult>> GetResultsByCompetitionAsync(int competitionId)
    {
        var results = _storage
            .Where(kvp => kvp.Key.StartsWith($"{competitionId}_"))
            .SelectMany(kvp => kvp.Value)
            .ToList();

        return Task.FromResult(results);
    }

    /// <inheritdoc />
    public Task<List<ContestantResult>> GetResultsByJudgeAsync(int competitionId, int judgeId)
    {
        var key = GetKey(competitionId, judgeId);
        if (_storage.TryGetValue(key, out var results))
        {
            return Task.FromResult(results);
        }
        return Task.FromResult(new List<ContestantResult>());
    }

    private int GetNextId()
    {
        lock (_idLock)
        {
            return _nextId++;
        }
    }

    private static void ValidateDto(CompetitionResultDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (dto.CompetitionId <= 0)
        {
            throw new ArgumentException("CompetitionId must be greater than 0", nameof(dto));
        }

        if (dto.JudgeId <= 0)
        {
            throw new ArgumentException("JudgeId must be greater than 0", nameof(dto));
        }

        if (dto.Results == null || dto.Results.Count == 0)
        {
            throw new ArgumentException("At least one result is required", nameof(dto));
        }

        var validScores = new[] { 0.0, 4.2, 4.3, 4.5, 10.0 };
        foreach (var result in dto.Results)
        {
            if (result.ContestantId <= 0)
            {
                throw new ArgumentException($"ContestantId must be greater than 0", nameof(dto));
            }

            if (!validScores.Contains(result.Score))
            {
                throw new ArgumentException(
                    $"Invalid score {result.Score} for contestant {result.ContestantId}. Valid scores are: 0 (No), 4.2 (Alt3), 4.3 (Alt2), 4.5 (Alt1), 10 (Yes)",
                    nameof(dto));
            }
        }
    }
}
