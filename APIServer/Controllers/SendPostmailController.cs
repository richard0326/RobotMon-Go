using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendPostmailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<SendPostmailController> _logger;

        public SendPostmailController(ILogger<SendPostmailController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<SendPostmailResponse> SendPostmailPost(SendPostmailRequest request)
        {
            var response = new SendPostmailResponse();

            var result = await _gameDb.SendPostmailAsync(request.ID, request.StarCount);
            if (result != ErrorCode.None)
            {
                response.Result = result;
                _logger.ZLogDebug($"{nameof(SendPostmailPost)} ErrorCode : {response.Result}");
                return response;
            }
            return response;
        }
    }
}