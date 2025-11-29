using Microsoft.AspNetCore.Mvc;

namespace CompetitionDB.Api;

using CompetitionDB.Models;
using CompetitionDB.Services;

/// <summary>
/// API endpoints for managing competition results from the Judge page.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CompetitionResultsController : ControllerBase
{
    private readonly ICompetitionResultService _resultService;
    private readonly ILogger<CompetitionResultsController> _logger;

    public CompetitionResultsController(
        ICompetitionResultService resultService,
        ILogger<CompetitionResultsController> logger)
    {
        _resultService = resultService;
        _logger = logger;
    }

    /// <summary>
    /// Submit new competition results from a judge.
    /// </summary>
    /// <param name="dto">The competition results to submit.</param>
    /// <returns>The saved results with generated IDs.</returns>
    /// <response code="201">Results created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="409">Results already exist for this competition/judge combination.</response>
    [HttpPost]
    [ProducesResponseType(typeof(List<ContestantResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateResults([FromBody] CompetitionResultDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation(
                "Creating results for competition {CompetitionId} from judge {JudgeId} with {ResultCount} results",
                dto.CompetitionId, dto.JudgeId, dto.Results.Count);

            var results = await _resultService.SaveResultsAsync(dto);
            return CreatedAtAction(
                nameof(GetResultsByJudge),
                new { competitionId = dto.CompetitionId, judgeId = dto.JudgeId },
                results);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Results already exist");
            return Conflict(new ProblemDetails
            {
                Title = "Results already exist",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    /// <summary>
    /// Update existing competition results from a judge.
    /// </summary>
    /// <param name="dto">The updated competition results.</param>
    /// <returns>The updated results.</returns>
    /// <response code="200">Results updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="404">No existing results found to update.</response>
    [HttpPut]
    [ProducesResponseType(typeof(List<ContestantResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateResults([FromBody] CompetitionResultDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation(
                "Updating results for competition {CompetitionId} from judge {JudgeId} with {ResultCount} results",
                dto.CompetitionId, dto.JudgeId, dto.Results.Count);

            var results = await _resultService.UpdateResultsAsync(dto);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Results not found");
            return NotFound(new ProblemDetails
            {
                Title = "Results not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    /// <summary>
    /// Get all results for a competition.
    /// </summary>
    /// <param name="competitionId">The competition identifier.</param>
    /// <returns>List of all contestant results for the competition.</returns>
    /// <response code="200">Results retrieved successfully.</response>
    [HttpGet("competition/{competitionId}")]
    [ProducesResponseType(typeof(List<ContestantResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResultsByCompetition(int competitionId)
    {
        _logger.LogInformation("Getting results for competition {CompetitionId}", competitionId);
        var results = await _resultService.GetResultsByCompetitionAsync(competitionId);
        return Ok(results);
    }

    /// <summary>
    /// Get results submitted by a specific judge for a competition.
    /// </summary>
    /// <param name="competitionId">The competition identifier.</param>
    /// <param name="judgeId">The judge identifier.</param>
    /// <returns>List of contestant results from the specified judge.</returns>
    /// <response code="200">Results retrieved successfully.</response>
    [HttpGet("competition/{competitionId}/judge/{judgeId}")]
    [ProducesResponseType(typeof(List<ContestantResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResultsByJudge(int competitionId, int judgeId)
    {
        _logger.LogInformation(
            "Getting results for competition {CompetitionId} from judge {JudgeId}",
            competitionId, judgeId);

        var results = await _resultService.GetResultsByJudgeAsync(competitionId, judgeId);
        return Ok(results);
    }
}
