using RestflowAPI.DTOs.AI;
using RestflowAPI.DTOs.AI.Internal;

namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface IDashboardInsightsService
    {
        Task<List<DashboardInsightDto>> GenerateInsightsAsync(
            CancellationToken cancellationToken);

        Task<List<DashboardInsightDto>> GetInsightsAsync(
        CancellationToken cancellationToken);
    }
}
