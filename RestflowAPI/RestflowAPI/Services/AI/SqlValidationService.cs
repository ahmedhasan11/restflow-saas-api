using RestflowAPI.ServiceInterfaces.AI;
using System.Text.RegularExpressions;

namespace RestflowAPI.Services.AI
{
    public class SqlValidationService : ISqlValidationService
    {
        private static readonly string[] Forbidden =
        {
        "INSERT",
        "UPDATE",
        "DELETE",
        "DROP",
        "ALTER",
        "TRUNCATE",
        "EXEC",
        "EXECUTE",
        "MERGE",
        "GRANT",
        "REVOKE",
        "CREATE"
    };

        public bool IsValid(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;

            sql = sql.Trim();

            if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (var keyword in Forbidden)
            {
                if (Regex.IsMatch(
                    sql,
                    $@"\b{keyword}\b",
                    RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
