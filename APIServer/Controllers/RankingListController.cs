using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;
using ZLogger;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RankingListController: ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<RankingListController> _logger;

        public RankingListController(ILogger<RankingListController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<RankingListResponse> RankingInfoPost(RankingListRequest request)
        {
            var response = new RankingListResponse();

            var result = await RankManager.CheckRankingInfo(request.PageIndex);
            var errorCode = result.Item1;
            var count = (Int32)result.Item2;
            var rankList = result.Item3;

            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.ZLogError($"{nameof(RankingInfoPost)} ErrorCode : {response.Result}");
                return response;
            }

            response.TotalSize = count;
            response.RankingIdList = rankList;
            return response;
        }
    }
}