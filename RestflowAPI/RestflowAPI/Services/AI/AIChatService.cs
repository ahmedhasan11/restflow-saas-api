using RestflowAPI.DTOs.AI;
using RestflowAPI.Repository.Interfaces.AI;
using RestflowAPI.ServiceInterfaces.AI;

namespace RestflowAPI.Services.AI
{
    public class AIChatService : IAIChatService
    {
        private readonly ISqlGenerationService _sqlGenerationService;

        private readonly ISqlValidationService _sqlValidationService;

        private readonly IDynamicQueryRepository _dynamicQueryRepository;

        private readonly IResponseSynthesisService _responseSynthesisService;

        public AIChatService(
            ISqlGenerationService sqlGenerationService,
            ISqlValidationService sqlValidationService,
            IDynamicQueryRepository dynamicQueryRepository,
            IResponseSynthesisService responseSynthesisService)
        {
            _sqlGenerationService = sqlGenerationService;
            _sqlValidationService = sqlValidationService;
            _dynamicQueryRepository = dynamicQueryRepository;
            _responseSynthesisService = responseSynthesisService;
        }

        public async Task<ChatMessageResponseDto> SendMessageAsync(
            ChatMessageRequestDto request,
            CancellationToken cancellationToken)
        {
            // Step 1
            var sql =
                await _sqlGenerationService.GenerateSqlAsync(
                    request.Message,
                    cancellationToken);

            // Step 2
            if (!_sqlValidationService.IsValid(sql))
            {
                return new ChatMessageResponseDto
                {
                    Success = false,
                    Response =
                        "عذراً، لم أتمكن من صياغة الاستعلام المناسب لسؤالك بدقة. يرجى إعادة صياغة السؤال بشكل أوضح."
                };
            }

            // Step 3
            var rows =
                await _dynamicQueryRepository.ExecuteSelectAsync(
                    sql,
                    cancellationToken);

            // Step 4
            var answer =
                await _responseSynthesisService.GenerateAnswerAsync(
                    request.Message,
                    rows,
                    cancellationToken);

            // Step 5
            return new ChatMessageResponseDto
            {
                Success = true,
                Response = answer
            };
        }

        public async Task<ChatMessageResponseDto>
ProcessMessageAsync(
    ChatMessageRequestDto request,
    CancellationToken cancellationToken)
        {
            var sql =
                await _sqlGenerationService.GenerateSqlAsync(
                    request.Message,
                    cancellationToken);

            if (!_sqlValidationService.IsValid(sql))
            {
                return new ChatMessageResponseDto
                {
                    Success = false,
                    Response =
                    "عذراً، لم أتمكن من صياغة الاستعلام المناسب لسؤالك."
                };
            }

            var rows =
                await _dynamicQueryRepository.ExecuteSelectAsync(
                    sql,
                    cancellationToken);

            var answer =
                await _responseSynthesisService.GenerateAnswerAsync(
                    request.Message,
                    rows,
                    cancellationToken);

            return new ChatMessageResponseDto
            {
                Success = true,
                Response = answer
            };
        }
    }
}
