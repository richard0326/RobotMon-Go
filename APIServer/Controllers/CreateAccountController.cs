using System;
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
    public class CreateAccountController : ControllerBase
    {
        // ControllerBase 객체는 ASPNET MVC에서 제공하는 객체로 모델 바인딩 하기 위해서 사용됨.
        private readonly IAccountDb _accountDb;
        private readonly IGameDb _gameDb;
        private readonly ILogger<CreateAccountController> _logger;
        
        public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDb accountDb, IGameDb gameDb)
        {
            _logger = logger;
            _accountDb = accountDb;
            _gameDb = gameDb;
        }
        
        [HttpPost]
        //public async Task<PkCreateAccountResponse> CreateAccountPost(PkCreateAccountRequest request, [FromServices] ICommonDb commonDb)
        // -> 서비스 여기서 받고 싶은 경우
        public async Task<CreateAccountResponse> CreateAccountPost(CreateAccountRequest request)
        {
            var response = new CreateAccountResponse();

            // PW 암호화 ( Salt + HashingPassword )
            var saltValue = Security.SaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, request.PW);

            var resultCode = await _accountDb.CreateAccountDataAsync(request.ID, hashingPassword, saltValue);
            if (resultCode != ErrorCode.None)
            {
                response.Result = resultCode;
                _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {resultCode}");
                return response;
            }
            
            // GameDB에 유저 기본 초기화 정보 세팅하기
            var errorCode = await _gameDb.InitUserGameInfoAsync(new TableUserGameInfo()
            {
                // 유저 초기 정보
                ID = request.ID,
                UserLevel = 1,
                UserExp = 0,
                StarPoint = 0,
                RankPoint = 0,
            });
            
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"CreateAccountPost ErrorCode 1 : {resultCode}");
                return response;
            }

            errorCode = await _gameDb.InitDailyCheckAsync(request.ID);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"CreateAccountPost ErrorCode 2 : {resultCode}");
                return response;
            }
            
            return response;
        }
    }
}