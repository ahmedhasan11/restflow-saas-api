using RestflowAPI.DTOs.AI.Internal;

namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface IDashboardPromptBuilder
    {
        string BuildInventoryPrompt(InventoryForecastData data);

        string BuildMenuPrompt(MenuEngineeringData data);

        string BuildPerformancePrompt(PerformanceSummaryData data);
    }
}
