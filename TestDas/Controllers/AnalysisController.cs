using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestDas.Models;
using TestDas.Services;
using TestDas.Services.DAL;

namespace TestDas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IValidator<AnalysisRequest> _validator;
    private readonly IParseService _parseService;
    private readonly IElementsRepository _elementsRepository;

    public AnalysisController(IValidator<AnalysisRequest> validator, IParseService parseService, IElementsRepository elementsRepository)
    {
        _validator = validator;
        _parseService = parseService;
        _elementsRepository = elementsRepository;
    }

    [HttpPost]
    [ProducesResponseType<AnalysisResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AnalysisResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadData([FromBody]AnalysisRequest analysisRequest)
    {
        var validationResult = await _validator.ValidateAsync(analysisRequest);

        if (!validationResult.IsValid)
        {
            var analysisResponse = new AnalysisResponse
            {
                Is_Error = 1,
                Error_Code = "ValidationException",
                Error_Message = string.Join(", ", validationResult.Errors
                    .Select(y => y.ErrorMessage))
            };
            return BadRequest(analysisResponse);
        }
            
        var parseResult = await _parseService.ParseAsync(analysisRequest);

        if(parseResult.Analysis.Is_Error == 1)
        {
            return BadRequest(parseResult.Analysis);
        }

        await _elementsRepository.AddRange(parseResult.Elements);

        return Ok(parseResult.Analysis);
    }
}