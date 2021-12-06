using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        protected readonly ICommonDb HandleCommonDb;
        
        public LoginController(ICommonDb commonDb)
        {
            HandleCommonDb = commonDb;
        }
        
        [HttpPost]
        public async Task<PkLoginResponse> LoginPost(PkLoginRequest request)
        {
            // 반환할 응답 객체
            var response = new PkLoginResponse();

            //using (HandleCommonDb)
            {
                // DB에서 로그인 시도에 대한 결과를 받아온다.
                var resultCode = await HandleCommonDb.GetLoginDataAsync(request.ID, request.PW);
                if (resultCode == ErrorCode.None)
                {
                    // 토큰 발행
                    response.Authtoken = ServerCommon.Security.AuthToken();
                }

                response.Result = resultCode;
            }

            return response;
        }
    }
    
    public class PkLoginRequest
    {
        public string ID { get; set; }
        public string PW { get; set; }
    }
    
    public class PkLoginResponse
    {
        public ServerCommon.ErrorCode Result { get; set; }
        public string Authtoken { get; set; }
    }
}