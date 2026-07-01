using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Services.AI.Prompts;

namespace RestflowAPI.Services.AI
{
    public class SqlGenerationService : ISqlGenerationService
    {
        private readonly SchemaContextBuilder _schemaBuilder;

        private readonly PromptBuilder _promptBuilder;

        private readonly ILLMService _llm;

        public SqlGenerationService(
            SchemaContextBuilder schemaBuilder,
            PromptBuilder promptBuilder,
            ILLMService llm)
        {
            _schemaBuilder = schemaBuilder;
            _promptBuilder = promptBuilder;
            _llm = llm;
        }

        public async Task<string> GenerateSqlAsync(
            string question,
            CancellationToken cancellationToken)
        {
            var schema =
                _schemaBuilder.Build();

            var prompt =
                _promptBuilder.BuildSqlPrompt(
                    schema,
                    question);

            var sql =
                await _llm.GenerateAsync(
                    SqlPrompt.System,
                    prompt,
                    cancellationToken);

            return sql.Trim();
        }
    }
}
