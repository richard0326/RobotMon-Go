using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserGameInfoController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<UserGameInfoController> _logger;
        
        public UserGameInfoController(ILogger<UserGameInfoController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }
        
        [HttpPost]
        public async Task<UserGameInfoResponse> GameInfoPost(UserGameInfoRequest request)
        {
            var response = new UserGameInfoResponse();
            
            // 유저의 게임 정보 가져오기
            var gameInfo = await _gameDb.GetUserGameInfoAsync(request.ID);
            if (gameInfo == null)
            {
                response.Result = ErrorCode.CreateAccountFailGetTable;
                _logger.ZLogDebug($"{nameof(GameInfoPost)} ErrorCode : {response.Result}");
                return response;
            }
            response.StarPoint = gameInfo.StarPoint;
            response.UserLevel = gameInfo.UserLevel;
            response.UserExp = gameInfo.UserExp;
            response.UpgradeCandy = gameInfo.UpgradeCandy;
            
            return response;
        }
    }
}