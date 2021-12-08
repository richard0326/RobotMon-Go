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
    public class UserInfoController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<UserInfoController> _logger;
        
        public UserInfoController(ILogger<UserInfoController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }
        
        [HttpPost]
        public async Task<UserInfoResponse?> UserInfoPost(UserInfoRequest request)
        {
            var response = new UserInfoResponse();
            
            // redis에서 로그인 유저 정보 받아오기... 없으면 로그인 성공한 유저가 아님.
            var userInfo = await RedisDB.GetUserInfo(request.ID);
            if (userInfo == null)
            {
                response.Result = ErrorCode.UserInfo_Fail_LoginFail;
                _logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;
            }
            
            // id, AuthToken 일치 여부 확인...
            if (userInfo.ID != request.ID || userInfo.AuthToken != request.AuthToken)
            {
                response.Result = ErrorCode.UserInfo_Fail_LoginFail;
                _logger.ZLogDebug($"UserInfoPost ErrorCode : {response.Result}");
                return response;   
            }
            
            // TODO DB 초기화 부분 또 실수하지 말고 확인하자...
            // db에서 유저 정보 받아오기...
            
            // 없는 경우 생성해서 넣어놓는다.
            
            // 유저 정보 전달하기
            
            return null;
        }
    }
}