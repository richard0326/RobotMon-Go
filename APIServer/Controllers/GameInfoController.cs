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
                response.Result = ErrorCode.UserInfoFailLoginFail;
                //_logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;
            }
            
            // id, AuthToken 일치 여부 확인...
            if (userInfo.AuthToken != request.AuthToken)
            {
                response.Result = ErrorCode.UserInfoFailWrongToken;
                //_logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;   
            }
            
            // TODO DB 초기화 부분 또 실수하지 말고 확인하자...
            // db에서 유저 정보 받아오기...
            // TODO 몬스터, 친구 목록, 몬스터 캔디는 나중에 추가할 예정
            // 일단은 1차원적인 정보만 DB로 붙어 받아온다.
            
            
            // 없는 경우 생성해서 데이터를 입력한다.
            
            // 유저 정보 전달하기
            response.RankPoint = 0;
            response.StarPoint = 0;
            
            return response;
        }
    }
}