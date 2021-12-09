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
            
            // redis에서 로그인 유저 정보 받아오기... 없으면 로그인 성공한 유저가 아님.
            var userInfo = await RedisDB.GetUserInfo(request.ID);
            if (userInfo == null)
            {
                response.Result = ErrorCode.UserGameInfoFailLoginFail;
                _logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;
            }
            
            // id, AuthToken 일치 여부 확인...
            if (userInfo.AuthToken != request.AuthToken)
            {
                response.Result = ErrorCode.UserGameInfoFailWrongToken;
                _logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;   
            }
            
            // 유저의 게임 정보 가져오기
            var gameInfo = await _gameDb.GetUserGameInfoAsync(request.ID);
            response.RankPoint = gameInfo.RankPoint;
            response.StarPoint = gameInfo.StarPoint;
            response.UserLevel = gameInfo.UserLevel;
            response.UserExp = gameInfo.UserExp;
            
            return response;
        }
    }
}