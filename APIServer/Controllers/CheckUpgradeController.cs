using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckUpgradeController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CheckUpgradeController> _logger;

        public CheckUpgradeController(ILogger<CheckUpgradeController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<CheckUpgradeResponse> CheckUpdatePost(CheckUpgradeRequest request)
        {
            var response = new CheckUpgradeResponse();

            var upgrade = DataStorage.GetMonsterUpgrade(request.UpgradeID);

            if (upgrade is null)
            {
                response.Result = ErrorCode.CheckUpdateFailNoMonsterExist;
                _logger.ZLogDebug($"{nameof(CheckUpdatePost)} ErrorCode : {response.Result}");
                return response;
            }

            response.UpgradeCost = upgrade.UpdateCost;
            response.StarCost = upgrade.StarCost;
            return response;
        }
    }
}