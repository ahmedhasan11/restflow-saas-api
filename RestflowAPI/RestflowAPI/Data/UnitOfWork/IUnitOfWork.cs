namespace RestflowAPI.Data.UnitOfWork
{
	public interface IUnitOfWork
	{
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
