using RestflowAPI.DTOs.AI;

namespace RestflowAPI.ServiceInterfaces.AI
{
    public interface IAIChatService
    {
        Task<ChatMessageResponseDto> SendMessageAsync(
        ChatMessageRequestDto request,
        CancellationToken cancellationToken);

        Task<ChatMessageResponseDto>
ProcessMessageAsync(
    ChatMessageRequestDto request,
    CancellationToken cancellationToken);
    }
}
