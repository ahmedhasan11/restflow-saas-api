using RestflowAPI.DTOs.AI.Internal;

namespace RestflowAPI.Repository.Interfaces.AI
{
    public interface IDashboardRepository
    {
        Task<InventoryForecastData> GetInventoryForecastAsync(
        CancellationToken cancellationToken);

        Task<MenuEngineeringData> GetMenuEngineeringAsync(
            CancellationToken cancellationToken);

        Task<PerformanceSummaryData> GetPerformanceSummaryAsync(
            CancellationToken cancellationToken);
    }
}
