using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController : ControllerBase
    {
        protected readonly ICommonDb HandleCommonDb;
        
        public CreateAccountController(ICommonDb commonDb)
        {
            HandleCommonDb = commonDb;
        }
        
        [HttpPost]
        //public async Task<PkCreateAccountResponse> CreateAccountPost(PkCreateAccountRequest request, [FromServices] ICommonDb commonDb)
        // -> 서비스 여기서 받고 싶은 경우
        public async Task<PkCreateAccountResponse> CreateAccountPost(PkCreateAccountRequest request)
        {
            var response = new PkCreateAccountResponse();
            response.Result = ErrorCode.None;

            // PW 암호화 ( Salt + HashingPassword )
            var saltValue = Security.SaltString();
            var hashingPassword = Security.MakeHashingPassWord(saltValue, request.PW);

            var resultCode = await HandleCommonDb.InsertCreateAccountDataAsync(request.ID, hashingPassword, saltValue);
            response.Result = resultCode;

            return response;
        }
    }
    
    public class PkCreateAccountRequest
    {
        public string ID { get; set; }
        public string PW { get; set; }
    }
    
    public class PkCreateAccountResponse
    {
        public ServerCommon.ErrorCode Result { get; set; }
    }
}