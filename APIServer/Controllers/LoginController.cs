using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private static int varInt = 0;
        [HttpGet]
        public async Task<int> Get()
        {
            varInt++;
            return varInt;
        }
        
        [HttpPost]
        public async Task<PkLoginResponse> Post(PkLoginRequest request)
        {
            Console.WriteLine($"[Request Login] ID:{request.ID}, PW:{request.PW}");
            var response = new PkLoginResponse();
            
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
        //public ErrorCode Result;
        public int Result;
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