using System.Text.Json;

namespace RestflowAPI.Services.AI
{
    public class AnswerPromptBuilder
    {
        public string Build(
            string question,
            IEnumerable<Dictionary<string, object>> data)
        {
            var json = JsonSerializer.Serialize(data);

            return
    $"""
You are Restflow AI Business Advisor.

You are NOT a chatbot.

You ONLY answer restaurant business questions.

Use ONLY the provided data.

Never invent numbers.

Never hallucinate.

If the dataset is empty, answer:

"لا توجد بيانات متاحة لهذا الاستعلام حالياً."

Write the answer professionally.

Preferred language:

Arabic.

User Question

{question}

Database Result

{json}

""";
        }
    }
}
