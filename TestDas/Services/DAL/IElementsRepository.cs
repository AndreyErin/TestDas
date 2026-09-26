using TestDas.Models;

namespace TestDas.Services.DAL
{
    public interface IElementsRepository
    {
        Task AddRange(List<Element> elements);
    }
}
