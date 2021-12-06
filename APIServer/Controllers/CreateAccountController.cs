using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController : ControllerBase
    {
        [HttpGet]
        public async Task<PkCreateAccountResponse> Get(PkCreateAccountRequest request)
        {
            var response = new PkCreateAccountResponse();
            response.Result = ErrorCode.None;
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