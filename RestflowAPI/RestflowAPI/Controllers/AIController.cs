using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestflowAPI.DTOs.AI.Internal;
using RestflowAPI.DTOs.AI;
using RestflowAPI.ServiceInterfaces.AI;
using RestflowAPI.Services.AI;

namespace RestflowAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize(Roles = "Owner")]
    public class AIController : ControllerBase
    {
        private readonly IAIChatService _chatService;

        private readonly IDashboardInsightsService _dashboardService;
        private readonly ILLMService _llm;

        public AIController(
            IAIChatService chatService,
            IDashboardInsightsService dashboardService, ILLMService llm)
        {
            _chatService = chatService;
            _dashboardService = dashboardService;
            _llm = llm;
        }

        [HttpPost("chat/message")]
        public async Task<ActionResult<ChatMessageResponseDto>> Chat(
            ChatMessageRequestDto request,
            CancellationToken cancellationToken)
        {
            var response =
                await _chatService.ProcessMessageAsync(
                    request,
                    cancellationToken);

            return Ok(response);
        }

        [HttpGet("dashboard-insights")]
        public async Task<ActionResult<List<DashboardInsightDto>>> Dashboard(
            CancellationToken cancellationToken)
        {
            var response =
                await _dashboardService.GetInsightsAsync(
                    cancellationToken);

            return Ok(response);
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var result = await _llm.GenerateAsync(
                "You are a helpful assistant.",
                "Say Hello",
                CancellationToken.None);

            return Ok(result);
        }
    }
}
