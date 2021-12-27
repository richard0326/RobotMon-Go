using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckCatchController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CheckCatchController> _logger;

        public CheckCatchController(ILogger<CheckCatchController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<CheckCatchResponse> CheckCatchPost(CheckCatchRequest request)
        {
            var response = new CheckCatchResponse();

            var result = await _gameDb.GetCatchListAsync(request.ID);
            var errorCode = result.Item1;
            var monsterList = result.Item2;

            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CheckCatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.MonsterInfoList = monsterList;
            return response;
        }
    }
}