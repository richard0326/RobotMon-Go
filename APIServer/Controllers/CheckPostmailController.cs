using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckPostmailController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CheckPostmailController> _logger;

        public CheckPostmailController(ILogger<CheckPostmailController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<CheckPostmailResponse> PostInfoPost(CheckPostmailRequest request)
        {
            var response = new CheckPostmailResponse();
            
            // db에서 Postmail된 정보 긁어오기
            var postmailInfo = await _gameDb.CheckPostmailAsync(request.ID);
            
            // 예외 상황이 발생한 경우
            if(postmailInfo is null)
            {
                response.Result = ErrorCode.PostmailFailException;
                return response;
            }
            
            if (postmailInfo.Count() == 0)
            {
                response.Result = ErrorCode.PostmailFailNoPostmail;
                return response;
            }

            response.PostmailInfo = postmailInfo;
            return response;
        }
    }
}