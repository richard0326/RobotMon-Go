using System;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;
using ZLogger;
using ApiServer.Model.Data;

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

            var createAccountResult = await _accountDb.CreateAccountDataAsync(request.ID, hashingPassword, saltValue);
            var errorCode = createAccountResult.Item1;
            var lastCreateIndex = createAccountResult.Item2;
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            // GameDB에 유저 기본 초기화 정보 세팅하기
            (errorCode, var lastGameInfoIndex) = await _gameDb.InitUserGameInfoAsync(request.ID, new UserGameInfo(1, 0, 0, 0));

            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            errorCode = await _gameDb.InitDailyCheckAsync(request.ID);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                // Rollback gameInfo 
                innerErrorCode = await _gameDb.RollbackInitUserGameInfoAsync(lastGameInfoIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;
            }

            errorCode = await RankManager.UpdateStarCount(request.ID, 0, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 계정 생성
                var innerErrorCode = await _accountDb.RollbackCreateAccountDataAsync(lastCreateIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }

                // Rollback gameInfo 
                innerErrorCode = await _gameDb.RollbackInitUserGameInfoAsync(lastGameInfoIndex);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }
                
                // Rollback DailyCheck
                innerErrorCode = await _gameDb.RollbackInitDailyCheckAsync(request.ID);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {innerErrorCode}");
                }
                
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CreateAccountPost)} ErrorCode : {response.Result}");
                return response;  
            }
            
            return response;
        }
    }
}