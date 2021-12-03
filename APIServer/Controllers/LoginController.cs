using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerCommon;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPut]
        public async Task<PkLoginResponse> Put(PkLoginRequest request)
        {
            // 반환할 응답 객체
            var response = new PkLoginResponse();
            response.Result = SErrorCode.None;
            
            // 암호화된 PW를 가져온다.
            // saltstring + SHA 암호화하여 hash 값을 얻는다.
            var saltValue = ServerCommon.Security.SaltString();
            var hashingPassword = ServerCommon.Security.MakeHashingPassWord(saltValue, request.PW);
            
            // DB에 확인
            if (false)
            {
                response.Result = SErrorCode.Login_Fail_NotUser;
                return response;
            }
            
            if (false)
            {
                response.Result = SErrorCode.Login_Fail_PW;
                return response;
            }

            // 토큰 발행
            response.Authtoken = ServerCommon.Security.AuthToken();

            return response;
        }
    }


    public class PkLoginRequest
    {
        public string ID;
        public string PW;
    }
    
    public class PkLoginResponse
    {
        public ServerCommon.SErrorCode Result;
        public string Authtoken;
    }

    class DBUserInfo
    {
        public UInt64 UID;
        public string PW;
        public string NickName;
        public string Salt;
    }
}