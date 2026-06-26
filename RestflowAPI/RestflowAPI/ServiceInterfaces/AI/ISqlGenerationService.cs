namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface ISqlGenerationService
    {
        Task<string> GenerateSqlAsync(
            string question,
            CancellationToken cancellationToken);
    }
}
