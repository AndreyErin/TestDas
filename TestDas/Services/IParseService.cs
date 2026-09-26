using TestDas.Models;

namespace TestDas.Services
{
    public interface IParseService
    {
        Task<ParseResult> ParseAsync(AnalysisRequest request);
    }
}
