using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAccountDb _accountDb;
        private readonly ILogger<LoginController> _logger;
        
        public LoginController(ILogger<LoginController> logger, IAccountDb accountDb)
        {
            _accountDb = accountDb;
            _logger = logger;
        }
        
        [HttpPost]
        public async Task<LoginResponse> LoginPost(LoginRequest request)
        {
            // 반환할 응답 객체
            var response = new LoginResponse();

            // Redis에 먼저 접근해서 AuthToken이 있는지 확인. 이미 존재한다면 이미 로그인되어 있는 상황이다.
            if (await RedisDB.CheckUserExist(request.ID))
            {
                response.Result = ErrorCode.Login_Fail_UserAlreadyExist;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            
            // Redis에 정보가 존재하지 않기에 AccountDB에서 로그인 시도에 대한 결과를 받아온다.
            var (password, salt) = (await _accountDb.GetLoginDataAsync(request.ID, request.PW))!;
            if (string.IsNullOrWhiteSpace(password))
            {
                response.Result = ErrorCode.Login_Fail_NoUserExist;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            
            // password 일치 여부 확인
            var hashingPassword = Security.MakeHashingPassWord(salt, request.PW);
            if (password != hashingPassword)
            {
                response.Result = ErrorCode.Login_Fail_WrongPassword;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            
            // 토큰 발행
            response.Authtoken = Security.AuthToken();

            // redis에 id, salt, AuthToken 값 저장
            if (!await RedisDB.SetUserInfo(request.ID, new RedisLoginData()
            {
                ID = request.ID,
                Salt = salt,
                AuthToken = response.Authtoken
            }))
            {
                response.Result = ErrorCode.Login_Fail_RedisError;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            
            // TODO 나중에 삭제되어야할 코드
            // 테스트 원활하게 하기 위해서 임시로 삭제하는 부분을 두었습니다.
            if (!await RedisDB.DelUserInfo(request.ID))
            {
                response.Result = ErrorCode.Login_Fail_RedisError;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            // 여기까지 삭제할 코드
            
            return response;
        }
    }
}