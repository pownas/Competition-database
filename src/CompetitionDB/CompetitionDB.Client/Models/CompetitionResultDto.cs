using System.ComponentModel.DataAnnotations;

namespace CompetitionDB.Client.Models;

/// <summary>
/// Data Transfer Object for submitting competition results.
/// </summary>
public class CompetitionResultDto
{
    /// <summary>
    /// The competition identifier.
    /// </summary>
    [Required]
    public int CompetitionId { get; set; }

    /// <summary>
    /// The judge identifier who is submitting results.
    /// </summary>
    [Required]
    public int JudgeId { get; set; }

    /// <summary>
    /// List of contestant scores.
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one result is required")]
    public List<ContestantScoreDto> Results { get; set; } = [];
}

/// <summary>
/// Data Transfer Object for a single contestant's score.
/// </summary>
public class ContestantScoreDto
{
    /// <summary>
    /// The contestant's identifier.
    /// </summary>
    [Required]
    public int ContestantId { get; set; }

    /// <summary>
    /// The contestant's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The score given to the contestant. Valid values are: 10 (Yes), 4.5 (Alt1), 4.3 (Alt2), 4.2 (Alt3), 0 (No).
    /// </summary>
    [Required]
    public double Score { get; set; }
}
