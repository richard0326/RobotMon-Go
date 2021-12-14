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
            var response = new DailyCheckResponse();

            // Update를 시도한다.
            _gameDb.TryDailyCheckAsync(request.ID);
            // Insert를 시도한다.
            
            
            
            return response;
        }
    }
}