using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    class EvolveController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<EvolveController> _logger;

        public EvolveController(ILogger<EvolveController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<EvolveResponse> EvolvePost(EvolveRequest request)
        {
            var response = new EvolveResponse();

            // 비용이 충분한지 알아야함.
            var result = DataStorage.GetMonsterEvolve(request.MonsterID);
            if (result is null)
            {
                // 잘못된 몬스터 ID
                response.Result = ErrorCode.EvolvePostFailNoMonsterId;
                _logger.ZLogDebug($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 비용 삭감 시도.
            var errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -result.CandyCount);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 기존의 Catch의 MonsterID를 update 시켜야함.
            errorCode = await _gameDb.EvolveCatchMonsterAsync(request.CatchID, result.EvolveMonsterID);
            if(errorCode != ErrorCode.None)
            {
                // Rollback
                var innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, result.CandyCount);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(EvolvePost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(EvolvePost)} ErrorCode : {response.Result}");
                return response;
            }

            response.EvolveMonsterID = result.EvolveMonsterID;
            return response;
        }
    }
}