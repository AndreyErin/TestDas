using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestDas.Models;
using TestDas.Services;
using TestDas.Services.DAL;

namespace TestDas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HtmlExtractionController : ControllerBase
{
    private readonly IValidator<HtmlExtractionRequest> _validator;
    private readonly IParseService _parseService;
    private readonly IElementsRepository _elementsRepository;

    public HtmlExtractionController(IValidator<HtmlExtractionRequest> validator, IParseService parseService, IElementsRepository elementsRepository)
    {
        _validator = validator;
        _parseService = parseService;
        _elementsRepository = elementsRepository;
    }

    [HttpPost]
    [ProducesResponseType<HtmlExtractionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HtmlExtractionResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<HtmlExtractionResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Extract([FromBody]HtmlExtractionRequest htmlExtractionRequest)
    {
        var validationResult = await _validator.ValidateAsync(htmlExtractionRequest);

        if (!validationResult.IsValid)
        {
            var extractionResponse = new HtmlExtractionResponse
            {
                IsError = 1,
                ErrorCode = "ValidationException",
                ErrorMessage = string.Join(", ", validationResult.Errors
                    .Select(y => y.ErrorMessage))
            };
            return BadRequest(extractionResponse);
        }
            
        var parseResult = await _parseService.ParseAsync(htmlExtractionRequest);

        if(parseResult.HtmlExtractionResponse.IsError == 1)
        {
            return BadRequest(parseResult.HtmlExtractionResponse);
        }

        await _elementsRepository.AddRange(parseResult.Elements);

        return Ok(parseResult.HtmlExtractionResponse);
    }
}