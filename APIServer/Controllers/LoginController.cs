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
            
            // Redis에 정보가 존재하지 않기에 AccountDB에서 로그인 시도에 대한 결과를 받아온다.
            var result = (await _accountDb.CheckPasswordAsync(request.ID, request.PW))!;
            if(result != ErrorCode.None)
            {
                response.Result = result;
                _logger.ZLogDebug($"{nameof(LoginPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 토큰 발행
            response.Authtoken = Security.AuthToken();

            // redis에 id, salt, AuthToken 값 저장
            if (!await RedisDB.SetUserInfo(request.ID, new RedisLoginData()
            {
                ID = request.ID,
                AuthToken = response.Authtoken
            }))
            {
                response.Result = ErrorCode.LoginFailRedisError;
                _logger.ZLogDebug($"LoginPost ErrorCode : {response.Result}");
                return response;
            }
            
            return response;
        }
    }
}