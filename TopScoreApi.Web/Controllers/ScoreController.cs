using MediatR;
using Microsoft.AspNetCore.Mvc;
using TopScoreApi.Application.Command;
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

    [HttpPost("upload-list")]
    public async Task<ActionResult<int>> UploadList([FromBody] List<ScoreDto> scores, CancellationToken cancellationToken)
    {
        if (scores == null || scores.Count == 0)
        {
            return BadRequest("Score list cannot be empty");
        }

        var insertedCount = await _mediator.Send(new AddScoresCommand(scores), cancellationToken);

        return Ok(new { Message = $"{insertedCount} item(s) uploaded successfully" });
    }

}