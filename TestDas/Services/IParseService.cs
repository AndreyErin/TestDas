using TestDas.Models;

namespace TestDas.Services
{
    public interface IParseService
    {
        Task<AnalysisResponse> ParseAsync(AnalysisRequest request);
    }
}
