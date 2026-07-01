namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface ILLMService
    {
        Task<string> GenerateAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken);
    }
}
