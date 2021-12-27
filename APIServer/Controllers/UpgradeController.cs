using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpgradeController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<UpgradeController> _logger;
        
        public UpgradeController(ILogger<UpgradeController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        // 수습 기간 프로젝트 임으로 실제로 강화하지는 않고 강화 비용만 차감하도록 합니다.
        [HttpPost]
        public async Task<UpgradeResponse> UpgradePost(UpgradeRequest request)
        {
            var response = new UpgradeResponse();
            
            // 유저에게 있는 starCount 감소 & 업그레이드 candy 감소
            
            
            return response;
        }
    }
}