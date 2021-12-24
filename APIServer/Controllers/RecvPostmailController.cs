using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecvPostmailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<RecvPostmailController> _logger;

        public RecvPostmailController(ILogger<RecvPostmailController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<RecvPostmailResponse> RecvPostmailPost(RecvPostmailRequest request)
        {
            var response = new RecvPostmailResponse();

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