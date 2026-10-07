using MediatR;
using Microsoft.AspNetCore.Mvc;
using TopScoreApi.Application.Commands;
using TopScoreApi.Application.Models;
using TopScoreApi.Application.Queries;

namespace TopScoreApi.Web.Controllers;

/// <summary>
/// The Score Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mediator"></param>
    public ScoreController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the top score from the database as well as the names with that score.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("top-score")]
    public async Task<ActionResult<TopScoreDto>> GetTopScores(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetTopScoreQuery(), cancellationToken);
    }

    /// <summary>
    /// Returns a person and their score if found. 
    /// </summary>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Uploads a list of scores to the database.
    /// Completes an upsert on the provided data.
    /// </summary>
    /// <param name="scores"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Uploads the contents of a CSV file to the database.
    /// Completes an upsert on the provided data.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload-csv")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UpsertScoresResultDto>> UploadCsv(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Please upload a valid CSV file.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file format.");
        }

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());

            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine) || headerLine.Split(',').Length != 3)
            {
                return BadRequest("Invalid header row.");
            }

            List<ScoreDto> scoreDtos = [];
            string? line;

            var rowNumber = 2;

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                if (values.Length == 3 && int.TryParse(values[2], out int score))
                {
                    scoreDtos.Add(new ScoreDto()
                    {
                        FirstName = values[0].Trim(),
                        LastName = values[1].Trim(),
                        Score = score
                    });
                }
                else
                {
                    Console.WriteLine($"Invalid row on line #{rowNumber}. Skipping...");
                }

                rowNumber++;
            }

            var result = await _mediator.Send(new AddScoresCommand(scoreDtos), cancellationToken);

            return Ok(new { Message = $"{result.InsertedCount} item(s) inserted successfully.\n{result.UpdatedCount} item(s) updated successfully." });

        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }

}