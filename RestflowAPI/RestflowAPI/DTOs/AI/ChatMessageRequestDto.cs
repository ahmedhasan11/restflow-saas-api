using System.ComponentModel.DataAnnotations;

namespace RestflowAPI.DTOs.AI
{
    public class ChatMessageRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }
}
