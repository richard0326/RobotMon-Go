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
        [HttpGet]
        public async Task<PkLoginResponse> Get(PkLoginRequest request)
        {
            Console.WriteLine(request.ID);
            Console.WriteLine(request.PW);
            
            // 반환할 응답 객체
            var response = new PkLoginResponse();
            response.Result = ErrorCode.None;
            
            // DB에 ID, PW 확인
            using (var connection = await MemoryManager.GetGameDBConnection())
            {
                var userInfo = await connection.QuerySingleOrDefaultAsync<DBUserInfo>(
                    @"select UID, PW, NickName, Salt from Users where ID = @id", new {id = request.ID});
                if (userInfo == null || string.IsNullOrEmpty(userInfo.PW))
                {
                    response.Result = ServerCommon.ErrorCode.login_Fail_NotUser;
                    return response;
                }
            
                // 암호화된 PW를 가져온다.
                // saltstring + SHA 암호화하여 hash 값을 얻는다.
                var saltValue = ServerCommon.Security.SaltString();
                var hashingPassword = ServerCommon.Security.MakeHashingPassWord(saltValue, request.PW);    
                if (userInfo.PW != hashingPassword)
                {
                    response.Result = ServerCommon.ErrorCode.login_Fail_PW;
                    return response;
                }
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
        public ServerCommon.ErrorCode Result { get; set; }
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