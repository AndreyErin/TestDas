using Dapper;
using Npgsql;
using TestDas.Models;

namespace TestDas.Services.DAL
{
    public class ElementRepository : IElementsRepository
    {
        private const string ConnectionString = @"Host=db;Port=5432;Database=testdas;Username=postgres;Password=postgres";

        public async Task AddRange(List<Element> elements)
        {
            const string sql = @"
            INSERT INTO elements (attribute_value, html_content)
            VALUES (@AttributeValue, @HtmlContent)";

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.ExecuteAsync(sql, elements);
        }
    }
}
