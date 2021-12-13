using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldMonsterController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<FieldMonsterController> _logger;
        
        public FieldMonsterController(ILogger<FieldMonsterController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<FieldMonsterResponse> FieldMonsterPost(FieldMonsterRequest request)
        {
            var response = new FieldMonsterResponse();
            
            var rand = new Random();
            var randValue = rand.Next(1, 7); // 기획 데이터 UID 1~6까지 존재함.
            var monster = DataStorage.GetMonsterInfo(randValue);
            response.MonsterID = randValue;
            response.Att = monster.Att;
            response.Def = monster.Def;
            response.Level = monster.Level;
            response.Name = monster.MonsterName;
            response.Type = monster.Type;
            response.HP = monster.HP;
            response.StarCount = monster.StarCount;
            
            return response;
        }
    }
}