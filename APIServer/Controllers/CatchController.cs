using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatchController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<CatchController> _logger;
        
        public CatchController(ILogger<CatchController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }
        
        // 수습 기간 프로젝트임으로 기능이 간단하게 구현되었습니다.
        [HttpPost]
        public async Task<CatchResponse> CatchPost(CatchRequest request)
        {
            var response = new CatchResponse();

            var rand = new Random();
            var randValue = rand.Next(1, 101); // 랜덤 확률. 1~100
            
            // 테스트 중이기 때문에 확률 100%
            if (randValue < 0)
            {
                response.Result = ErrorCode.CatchFail;
                return response;
            }

            var monster = DataStorage.GetMonsterInfo(request.MonsterID);
            if (monster == null)
            {
                response.Result = ErrorCode.DataStorageReadMonsterFail;
                _logger.ZLogDebug($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            // 현재 날짜 - 시간 정보를 원하는 경우 DateTime.Now를 사용할 것.
            response.Date = DateTime.Today;
            
            // DB에 잡은 정보 저장
            var result = await _gameDb.SetCatchAsync(new TableCatch()
            {
                MonsterID = request.MonsterID,
                UserID = request.ID,
                CatchTime = response.Date
            });

            var errorCode = result.Item1;
            var catchId = result.Item2;
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                if (errorCode != ErrorCode.CatchFail)
                {
                    _logger.ZLogDebug($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                }

                return response;
            }
            
            // 기획 데이터를 읽어왔지만 사용하지 않고, 랜덤하게 StarCount 주는 방식으로 수정
            monster.StarCount = rand.Next(100, 1001);
            
            // 유저의 잡은 정보 수정
            errorCode = await RankManager.UpdateStarCount(request.ID, monster.StarCount, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                var insideErrorCode = await _gameDb.RollbackSetCatchAsync(catchId);
                if (insideErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(CatchPost)} ErrorCode : {insideErrorCode}");
                }
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(CatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.CatchID = catchId;
            response.StarCount = monster.StarCount;
            response.UpgradeCandy = monster.UpgradeCount;
            response.MonsterID = request.MonsterID;
            return response;
        }
    }
}