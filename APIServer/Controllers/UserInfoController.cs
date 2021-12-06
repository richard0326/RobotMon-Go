using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController : ControllerBase
    {
        [HttpPost]
        public async Task<PkLoginResponse> UserInfoPost(PkLoginRequest request)
        {
            return null;
        }
    }
}