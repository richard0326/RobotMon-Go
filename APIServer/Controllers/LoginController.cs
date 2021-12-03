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
        public async Task<PkLoginResponse> Get(PkLoginRequest request)
        {
            Console.WriteLine(request.ID);
            Console.WriteLine(request.PW);
            
            // 반환할 응답 객체
            var response = new PkLoginResponse();
            //response.Result = SErrorCode.None;
            response.Result = 0;
            
            // 암호화된 PW를 가져온다.
            // saltstring + SHA 암호화하여 hash 값을 얻는다.
            var saltValue = ServerCommon.Security.SaltString();
            var hashingPassword = ServerCommon.Security.MakeHashingPassWord(saltValue, request.PW);

            string ez = request.ID;
            // DB에 확인
            if (false)
            {
                //response.Result = SErrorCode.login_Fail_NotUser;
                return response;
            }
            
            if (false)
            {
                //response.Result = SErrorCode.login_Fail_PW;
                return response;
            }

            // 토큰 발행
            response.Authtoken = ServerCommon.Security.AuthToken();

            Console.WriteLine(response.Authtoken);
            
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
        //public ServerCommon.SErrorCode Result;
        public int Result { get; set; }
        public string Authtoken { get; set; }
    }

    class DBUserInfo
    {
        public UInt64 UID;
        public string PW;
        public string NickName;
        public string Salt;
    }
}