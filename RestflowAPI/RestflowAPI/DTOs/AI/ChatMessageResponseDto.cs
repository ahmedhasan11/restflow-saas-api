namespace RestflowAPI.DTOs.AI
{
    public class ChatMessageResponseDto
    {
        public bool Success { get; set; }

        public string Response { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
