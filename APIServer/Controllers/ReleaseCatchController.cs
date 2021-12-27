using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReleaseCatchController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<ReleaseCatchController> _logger;

        public ReleaseCatchController(ILogger<ReleaseCatchController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<ReleaseCatchResponse> ReleaseCatchPost(ReleaseCatchRequest request)
        {
            var response = new ReleaseCatchResponse();

            var result = await _gameDb.DelCatchAsync(request.ReleaseID);
            var errorCode = result.Item1;
            var catchID = result.Item2;
            var monsterID = result.Item3;
            var catchDate = result.Item4;

            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(ReleaseCatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            var monster = DataStorage.GetMonsterInfo(monsterID);
            if (monster is null)
            {
                _gameDb.RollbackDelCatchAsync(request.ID, monsterID, catchDate);
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(ReleaseCatchPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            response.UpgradeCandy = monster.UpgradeCount;
            return response;
        }
    }
}