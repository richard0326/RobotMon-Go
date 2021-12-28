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
                response.Result = ErrorCode.UpgradePostFailNoMonsterID;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }

            // 유저가 '별의모래'에 대한 값을 지불 가능한지 확인한다.
            var totalStarCount = monsterUpgrade.StarCost * request.UpgradeSize;
            var errorCode = await RankManager.UpdateStarCount(request.ID, -totalStarCount, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                // 업데이트 실패
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 유저가 'UpgradeCandy'에 대한 값을 지불 가능한지 확인한다.
            var totalUpgradeCost = monsterUpgrade.UpdateCost * request.UpgradeSize;
            errorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, totalUpgradeCost);
            if (errorCode != ErrorCode.None)
            {
                // Rollback Update 별의모래
                var innerErrorCode = await RankManager.RollbackUpdateStarCount(request.ID, -totalStarCount, _gameDb);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 원래라면 Monster를 강화해야하지만, 유저를 강화시키는 것으로 대신함.
            // 유저 레벨업
            var gainExp = monsterUpgrade.Exp * request.UpgradeSize;
            errorCode = await _gameDb.UpdateUserExpAsync(request.ID, gainExp);
            if (errorCode != ErrorCode.None)
            {
                // Rollback Update 별의모래
                var innerErrorCode = await RankManager.RollbackUpdateStarCount(request.ID, -totalStarCount, _gameDb);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }
                
                // Rollback Upgrade 비용
                innerErrorCode = await _gameDb.UpdateUpgradeCostAsync(request.ID, -totalUpgradeCost);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(UpgradePost)} ErrorCode : {response.Result}");
                return response;
            }
            
            return response;
        }
    }
}