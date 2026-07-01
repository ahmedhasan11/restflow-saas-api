using Dapper;
using Microsoft.EntityFrameworkCore;
using RestflowAPI.Data;
using RestflowAPI.Repository.Interfaces.AI;
using System.Data;

namespace RestflowAPI.Repository.AI
{
    public class DynamicQueryRepository
    : IDynamicQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public DynamicQueryRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dictionary<string, object>>> ExecuteSelectAsync(
       string sql,
       CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query is empty.");

            if (!sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only SELECT queries are allowed.");

            using var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(
                sql,
                cancellationToken: cancellationToken,
                commandTimeout: 5);

            var rows = await connection.QueryAsync(command);

            List<Dictionary<string, object>> result = new();

            foreach (IDictionary<string, object> row in rows)
            {
                result.Add(new Dictionary<string, object>(row));
            }

            return result;
        }
    }
}
