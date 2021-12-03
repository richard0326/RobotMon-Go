using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [ApiController]
        [Route("[controller]")]
        public class LoginController : ControllerBase
        {
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
}