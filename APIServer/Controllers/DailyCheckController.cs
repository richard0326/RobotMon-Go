using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

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
        
        // 보상은 간단히 하기 위해서 요일별로 excel에 지정되어있는 기획데이터의 StarPoint만 주고 있음.
        [HttpPost]
        public async Task<DailyCheckResponse> DailyCheckPost(DailyCheckRequest request)
        {
            var response = new DailyCheckResponse();
            
            // DB를 조회헤서 유저의 출석 체크 성공 여부를 알려줍니다.
            var result = await _gameDb.TryDailyCheckAsync(request.ID);
            var errorCode = result.Item1;
            var prevDate = result.Item2;
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            // 보상 주기 - 월요일부터 일요일까지 요일 별로 보상이 있음.
            // 일요일 0, 월요일 1, 화요일 2, ...
            var dailyInfo = DataStorage.GetDailyInfo((int) DateTime.Today.DayOfWeek + 1);
            if (dailyInfo is null)
            {
                response.Result = ErrorCode.DailyCheckFailNoStoredData;
                _logger.ZLogDebug($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            errorCode = await RankManager.UpdateStarCount(request.ID, dailyInfo.StarCount, _gameDb);
            if (errorCode != ErrorCode.None)
            {
                // Rollback 시도
                var innerErrorCode = await _gameDb.RollbackDailyCheckAsync(request.ID, prevDate);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.ZLogDebug($"{nameof(DailyCheckPost)} ErrorCode : {innerErrorCode}");
                }
                response.Result = errorCode;
                _logger.ZLogDebug($"{nameof(DailyCheckPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            response.StarCount = dailyInfo.StarCount;
            
            return response;
        }
    }
}