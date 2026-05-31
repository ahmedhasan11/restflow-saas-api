namespace RestflowAPI.Repository.Interfaces.Reports
{
	public interface IReportsRepository
	{
		Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
	}
}
