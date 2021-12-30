using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecvMailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<RecvMailController> _logger;

        public RecvMailController(ILogger<RecvMailController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<RecvMailResponse> RecvPostmailPost(RecvMailRequest request)
        {
            var response = new RecvMailResponse();

            var postmailInfo = await _gameDb.RecvPostmailAsync(request.ID, request.PostmailID);
            var errorCode = postmailInfo.Item1;
            var starCount = postmailInfo.Item2;
            var date = postmailInfo.Item3;
            
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(RecvPostmailPost)} ErrorCode : {response.Result}");
                return response;
            }

            errorCode = await RankManager.UpdateStarCount(request.ID, starCount, _gameDb);
            if(errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.RollbackRecvPostmailAsync(request.ID, starCount, date);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(RecvPostmailPost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(RecvPostmailPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.StarCount = starCount;

            return response;
        }
    }
}