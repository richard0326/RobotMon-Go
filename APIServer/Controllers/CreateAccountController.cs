using System;
using System.Threading.Tasks;
using ApiServer.Model;
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
        private readonly ILogger<CreateAccountController> _logger;
        
        public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDb accountDb)
        {
            _logger = logger;
            _accountDb = accountDb;
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
                _logger.ZLogDebug($"CreateAccountPost ErrorCode : {resultCode}");
                return response;
            }

            return response;
        }
    }
}