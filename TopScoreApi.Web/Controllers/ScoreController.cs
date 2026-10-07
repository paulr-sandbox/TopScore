using MediatR;
using Microsoft.AspNetCore.Mvc;
using TopScoreApi.Application.Commands;
using TopScoreApi.Application.Models;
using TopScoreApi.Application.Queries;

namespace TopScoreApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ISender _mediator;

    public ScoreController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("top-score")]
    public async Task<ActionResult<TopScoreDto>> GetTopScores(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetTopScoreQuery(), cancellationToken);
    }

    [HttpGet("person")]
    public async Task<ActionResult<ScoreDto>> GetPersonScore([FromQuery] string FirstName, [FromQuery] string LastName, CancellationToken cancellationToken)
    {
        var score = await _mediator.Send(new GetPersonScoreQuery(FirstName, LastName), cancellationToken);

        if (score is null)
        {
            return NotFound(new { Message = $"Person with name '{FirstName} {LastName}' was not found." });
        }

        return Ok(score);
    }

    [HttpPost("upload-list")]
    public async Task<ActionResult<UpsertScoresResultDto>> UploadList([FromBody] List<ScoreDto> scores, CancellationToken cancellationToken)
    {
        if (scores == null || scores.Count == 0)
        {
            return BadRequest("Score list cannot be empty");
        }

        var result = await _mediator.Send(new AddScoresCommand(scores), cancellationToken);

        return Ok(new { Message = $"{result.InsertedCount} item(s) inserted successfully.\n{result.UpdatedCount} item(s) updated successfully." });
    }

}