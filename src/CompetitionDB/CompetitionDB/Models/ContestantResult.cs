namespace CompetitionDB.Models;

/// <summary>
/// Represents a contestant's result in a competition round.
/// </summary>
public class ContestantResult
{
    /// <summary>
    /// Unique identifier for the result.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The contestant's identifier.
    /// </summary>
    public int ContestantId { get; set; }

    /// <summary>
    /// The contestant's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The score given by the judge.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// The competition round identifier.
    /// </summary>
    public int CompetitionId { get; set; }

    /// <summary>
    /// The identifier of the judge who submitted the score.
    /// </summary>
    public int JudgeId { get; set; }

    /// <summary>
    /// Timestamp when the result was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
