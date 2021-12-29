using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckEvolveController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CheckEvolveController> _logger;

        public CheckEvolveController(ILogger<CheckEvolveController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<CheckEvolveResponse> CheckEvolvePost(CheckEvolveRequest request)
        {
            var response = new CheckEvolveResponse();

            var result = DataStorage.GetMonsterEvolve(request.MonsterID);
            if (result is null)
            {
                // 잘못된 몬스터 ID
                response.Result = ErrorCode.CheckEvolvePostFailNoMonsterId;
                _logger.ZLogDebug($"{nameof(CheckEvolvePost)} ErrorCode : {response.Result}");
                return response;
            }

            response.UpgradeCandy = result.CandyCount;
            return response;
        }
    }
}