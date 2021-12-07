using System.Threading.Tasks;
using ApiServer.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        // TODO 유저 정보 받기 기능 구현
        [HttpPost]
        public async Task<LoginResponse> UserInfoPost(LoginRequest request)
        {
            return null;
        }
    }
}