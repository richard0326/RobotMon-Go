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

            if (postmailInfo.Item1 != ErrorCode.None)
            {
                response.Result = postmailInfo.Item1;
                _logger.ZLogDebug($"{nameof(RecvPostmailPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.StarCount = postmailInfo.Item2;

            return response;
        }
    }
}