namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface IResponseSynthesisService
    {
        Task<string> GenerateAnswerAsync(
        string userQuestion,
        IEnumerable<Dictionary<string, object>> sqlResult,
        CancellationToken cancellationToken);
    }
}
