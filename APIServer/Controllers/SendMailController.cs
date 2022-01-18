using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;


namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendMailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<SendMailController> _logger;

        public SendMailController(ILogger<SendMailController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<SendMailResponse> SendMailPost(SendMailRequest request)
        {
            var response = new SendMailResponse();

            var result = await _gameDb.SendMailAsync(request.sendID, request.StarCount);
            var errorCode = result.Item1;
            var lastInsertId = result.Item2;
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.LogError($"{nameof(SendMailPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 원래 유저의 정보에서 StarCount를 차감합니다.
            errorCode = await UpdateStarCountAsync(request, lastInsertId);
            if (errorCode != ErrorCode.None)
            {                
                response.Result = errorCode;
                _logger.LogError($"{nameof(SendMailPost)} ErrorCode : {response.Result}");
                return response;
            }
            return response;
        }

        private async Task<ErrorCode> UpdateStarCountAsync(SendMailRequest request, Int64 lastInsertId)
        {
            // 원래 유저의 정보에서 StarCount를 차감합니다.
            var errorCode = await RankManager.UpdateStarCount(request.ID, -request.StarCount, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.RollbackSendMailAsync(lastInsertId);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.LogError($"{nameof(SendMailPost)} ErrorCode : {innerErrorCode}");
                }

                return errorCode;
            }
            return ErrorCode.None;
        }
    }
}