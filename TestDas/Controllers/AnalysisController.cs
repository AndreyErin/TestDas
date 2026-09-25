using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestDas.Models;

namespace TestDas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IValidator<AnalysisRequest> _validator;

    public AnalysisController(IValidator<AnalysisRequest> validator)
    {
        _validator = validator;
    }

    /// <response code="200">Успешная обработка запроса</response>
    /// <response code="400">Некорректные данные запроса</response>
    [HttpPost]
    [ProducesResponseType<AnalysisResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AnalysisResponse>(StatusCodes.Status400BadRequest)]
    public IActionResult UploadData([FromBody]AnalysisRequest analysisRequest)
    {
        var validationResult = _validator.Validate(analysisRequest);

        if (!validationResult.IsValid)
        {
            var analysisResponse = new AnalysisResponse
            {
                IsError = 1,
                ErrorCode = "ValidationException",
                ErrorMessage = string.Join(", ", validationResult.Errors
                    .Select(y => y.ErrorMessage))
            };
            return BadRequest(analysisResponse);
        }
            


        return Ok("Фсё ОК!");
    }
}