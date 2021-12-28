using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

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

            var monsterUpgrade = DataStorage.GetMonsterUpgrade(request.UpgradeID);
            if (monsterUpgrade == null)
            {
                // 잘못된 몬스터 ID
                return response;
            }

            // 유저가 값을 지불 가능한지 확인한다.

            var errorCode = await RankManager.UpdateStarCount(request.ID, monsterUpgrade.StarCost, _gameDb);
            
            // 코드 진행 중...
            
            //
            
            var gainExp = monsterUpgrade.Exp * request.UpgradeSize;
            
            // 원래라면 Monster를 강화해야하지만, 유저를 강화시키는 것으로 대신함.
            errorCode = await _gameDb.UpdateUserExpAsync(request.ID, gainExp);
            
            
            return response;
        }
    }
}