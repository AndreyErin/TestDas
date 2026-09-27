using TestDas.Models;

namespace TestDas.Services
{
    public interface IParseService
    {
        Task<ParseResult> ParseAsync(HtmlExtractionRequest request);
    }
}
