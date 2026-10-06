using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<ActionResult<TopScoreDto>> GetTopScores()
    {
        return await _mediator.Send(new GetTopScoreQuery());
    }
}