namespace RestflowAPI.Repository.Interfaces.AI
{
    public interface IDynamicQueryRepository
    {
        Task<IEnumerable<Dictionary<string, object>>> ExecuteSelectAsync(
        string sql,
        CancellationToken cancellationToken);
    }
}
