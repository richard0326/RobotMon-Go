using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyCheckController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<DailyCheckController> _logger;
        
        public DailyCheckController(ILogger<DailyCheckController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<DailyCheckResponse> DailyCheckPost(DailyCheckRequest request)
        {
            // 내부에서 db 처리 후 보상까지 주고 있습니다.
            return await _gameDb.TryDailyCheckAsync(request.ID);
        }
    }
}