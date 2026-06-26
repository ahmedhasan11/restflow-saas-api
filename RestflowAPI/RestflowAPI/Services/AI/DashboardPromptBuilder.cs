using RestflowAPI.DTOs.AI.Internal;
using RestflowAPI.ServiceInterfaces.AI;
using System.Text.Json;

namespace RestflowAPI.Services.AI
{
    public class DashboardPromptBuilder : IDashboardPromptBuilder
    {
        public string BuildInventoryPrompt(InventoryForecastData data)
        {
            return
$"""
You are Restflow AI.

Generate a professional restaurant inventory insight.

Rules:

- Respond in Arabic.
- Don't invent numbers.
- Use only the provided metrics.
- If there isn't enough data, politely say there isn't enough data.

Inventory Metrics

{JsonSerializer.Serialize(data)}
""";
        }

        public string BuildMenuPrompt(MenuEngineeringData data)
        {
            return
$"""
You are Restflow AI.

Generate Menu Engineering recommendations.

Rules

- Arabic only.
- Recommend combos.
- Recommend promoting slow-selling products.
- Do not invent statistics.

Menu Metrics

{JsonSerializer.Serialize(data)}
""";
        }

        public string BuildPerformancePrompt(PerformanceSummaryData data)
        {
            return
$"""
You are Restflow AI.

Generate an executive business summary.

Rules

- Arabic only.
- Explain today's performance.
- Mention revenue.
- Mention order types.
- Keep response concise.

Performance Metrics

{JsonSerializer.Serialize(data)}
""";
        }
    }
}
