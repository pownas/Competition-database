namespace CompetitionDB.Services;

using CompetitionDB.Models;

/// <summary>
/// Interface for competition result storage operations.
/// </summary>
public interface ICompetitionResultService
{
    /// <summary>
    /// Saves competition results to storage.
    /// </summary>
    /// <param name="dto">The competition results to save.</param>
    /// <returns>The saved results with generated IDs.</returns>
    Task<List<ContestantResult>> SaveResultsAsync(CompetitionResultDto dto);

    /// <summary>
    /// Updates existing competition results.
    /// </summary>
    /// <param name="dto">The competition results to update.</param>
    /// <returns>The updated results.</returns>
    Task<List<ContestantResult>> UpdateResultsAsync(CompetitionResultDto dto);

    /// <summary>
    /// Gets all results for a specific competition.
    /// </summary>
    /// <param name="competitionId">The competition identifier.</param>
    /// <returns>List of contestant results for the competition.</returns>
    Task<List<ContestantResult>> GetResultsByCompetitionAsync(int competitionId);

    /// <summary>
    /// Gets all results submitted by a specific judge for a competition.
    /// </summary>
    /// <param name="competitionId">The competition identifier.</param>
    /// <param name="judgeId">The judge identifier.</param>
    /// <returns>List of contestant results for the judge.</returns>
    Task<List<ContestantResult>> GetResultsByJudgeAsync(int competitionId, int judgeId);
}
