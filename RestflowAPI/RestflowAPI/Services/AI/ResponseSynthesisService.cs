using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Services.AI.Prompts;

namespace RestflowAPI.Services.AI
{
    public class ResponseSynthesisService
    : IResponseSynthesisService
    {
        private readonly ILLMService _llm;

        private readonly AnswerPromptBuilder _promptBuilder;

        public ResponseSynthesisService(
            ILLMService llm,
            AnswerPromptBuilder promptBuilder)
        {
            _llm = llm;
            _promptBuilder = promptBuilder;
        }

        public async Task<string> GenerateAnswerAsync(
            string userQuestion,
            IEnumerable<Dictionary<string, object>> sqlResult,
            CancellationToken cancellationToken)
        {
            if (sqlResult == null || !sqlResult.Any())
            {
                return "لا توجد بيانات متاحة لهذا الاستعلام حالياً.";
            }

            var prompt =
                _promptBuilder.Build(
                    userQuestion,
                    sqlResult);

            var answer =
                await _llm.GenerateAsync(
                    ResponsePrompt.System,
                    prompt,
                    cancellationToken);

            return answer.Trim();
        }
    }
}
