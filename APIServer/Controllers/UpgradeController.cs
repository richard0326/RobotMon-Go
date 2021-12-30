using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpgradeController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<UpgradeController> _logger;
        
        public UpgradeController(ILogger<UpgradeController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        // 수습 기간 프로젝트 임으로 실제로 몬스터 강화하지는 않고 유저 경험치를 주는 방식으로 진행.
        [HttpPost]
        public async Task<UpgradeResponse> UpgradePost(UpgradeRequest request)
        {
            var response = new UpgradeResponse();

            // 몬스터에 대한 기획 데이터를 가져온다.
            var monsterUpgrade = DataStorage.GetMonsterUpgrade(request.UpgradeID);
            if (monsterUpgrade == null)
            {
                // 잘못된 몬스터 ID
                response.Result = ErrorCode.UpgradePostFailNoMonsterId;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 유저가 'UpgradeCandy'에 대한 값을 지불 가능한지 확인한다.
            var totalUpgradeCost = monsterUpgrade.UpdateCost * request.UpgradeSize;
            var errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, totalUpgradeCost);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 유저가 '별의모래'에 대한 값을 지불 가능한지 확인한다.
            var totalStarCount = monsterUpgrade.StarCost * request.UpgradeSize;
            errorCode = await RankManager.UpdateStarCount(request.ID, -totalStarCount, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                var innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -totalUpgradeCost);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }

                // 업데이트 실패
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 몬스터 cp 증가하기!

            //
            
            
            return response;
        }
    }
}