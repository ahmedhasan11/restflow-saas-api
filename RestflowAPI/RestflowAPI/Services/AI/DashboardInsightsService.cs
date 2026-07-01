using RestflowAPI.DTOs.AI;
using RestflowAPI.DTOs.AI.Internal;
using RestflowAPI.Repository.AI;
using RestflowAPI.Repository.Interfaces.AI;
using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Services.AI.Prompts;

namespace RestflowAPI.Services.AI
{
    public class DashboardInsightsService : IDashboardInsightsService
    {
        private readonly IDashboardRepository _repository;
        private readonly IDashboardPromptBuilder _builder;
        private readonly ILLMService _llm;
        public DashboardInsightsService(IDashboardRepository repository, IDashboardPromptBuilder builder, ILLMService llm)
        {
            _repository = repository;
            _builder = builder;
            _llm = llm;
        }
        public async Task<List<DashboardInsightDto>> GenerateInsightsAsync(CancellationToken cancellationToken)
        {
            var result = new List<DashboardInsightDto>();

            var inventory =
                await _repository.GetInventoryForecastAsync(cancellationToken);

            var inventoryPrompt =
                _builder.BuildInventoryPrompt(inventory);

            var inventoryAnswer =
                await _llm.GenerateAsync("", inventoryPrompt, cancellationToken);

            result.Add(new DashboardInsightDto
            {
                Category = "Inventory",
                Insight = inventoryAnswer
            });

            var menu =
                await _repository.GetMenuEngineeringAsync(cancellationToken);

            var menuPrompt =
                _builder.BuildMenuPrompt(menu);

            var menuAnswer =
                await _llm.GenerateAsync("", menuPrompt, cancellationToken);

            result.Add(new DashboardInsightDto
            {
                Category = "Menu",
                Insight = menuAnswer
            });

            var performance =
                await _repository.GetPerformanceSummaryAsync(cancellationToken);

            var performancePrompt =
                _builder.BuildPerformancePrompt(performance);

            var performanceAnswer =
                await _llm.GenerateAsync("", performancePrompt, cancellationToken);

            result.Add(new DashboardInsightDto
            {
                Category = "Performance",
                Insight = performanceAnswer
            });

            return result;
        }

        public async Task<List<DashboardInsightDto>> GetInsightsAsync(
        CancellationToken cancellationToken)
        {
            List<DashboardInsightDto> insights = new();

            // Inventory
            var inventory =
                await _repository.GetInventoryForecastAsync(
                    cancellationToken);

            var inventoryPrompt =
                _builder.BuildInventoryPrompt(inventory);

            var inventoryAnswer =
                await _llm.GenerateAsync(DashboardPrompt.System,
                    inventoryPrompt,
                    cancellationToken);

            insights.Add(new DashboardInsightDto
            {
                Category = "Inventory",
                Insight = inventoryAnswer
            });

            // Menu Engineering
            var menu =
                await _repository.GetMenuEngineeringAsync(
                    cancellationToken);

            var menuPrompt =
                _builder.BuildMenuPrompt(menu);

            var menuAnswer =
                await _llm.GenerateAsync(DashboardPrompt.System,
                    menuPrompt,
                    cancellationToken);

            insights.Add(new DashboardInsightDto
            {
                Category = "Menu",
                Insight = menuAnswer
            });

            // Performance
            var performance =
                await _repository.GetPerformanceSummaryAsync(
                    cancellationToken);

            var performancePrompt =
                _builder.BuildPerformancePrompt(performance);

            var performanceAnswer =
                await _llm.GenerateAsync(DashboardPrompt.System,
                    performancePrompt,
                    cancellationToken);

            insights.Add(new DashboardInsightDto
            {
                Category = "Performance",
                Insight = performanceAnswer
            });

            return insights;
        }
    }
}
