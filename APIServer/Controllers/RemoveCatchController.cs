﻿using ApiServer.Model;
using ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using ServerCommon;


namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemoveCatchController : ControllerBase
    {
        private readonly IGameDb _gameDb;
        private readonly ILogger<RemoveCatchController> _logger;

        public RemoveCatchController(ILogger<RemoveCatchController> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        [HttpPost]
        public async Task<RemoveCatchResponse> RemoveCatchPost(RemoveCatchRequest request)
        {
            var response = new RemoveCatchResponse();

            var (errorCode, catchID, monsterID, catchDate, combatPoint) = await _gameDb.DelCatchAsync(request.RemoveID);

            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.LogError($"{nameof(RemoveCatchPost)} ErrorCode : {response.Result}");
                return response;
            }

            // monsterID를 받아서 찾는다.
            (errorCode, var monster) = await GetMonsterInfoAsync(monsterID, request.ID, catchDate);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                _logger.LogError($"{nameof(RemoveCatchPost)} ErrorCode : {response.Result}");
                return response;
            }
            
            response.UpgradeCandy = monster.UpgradeCount;
            return response;
        }

        private async Task<Tuple<ErrorCode, Monster>> GetMonsterInfoAsync(Int64 monsterID, string rollbackID, DateTime rollbackDate)
        {
            // monsterID를 받아서 찾는다.
            var monster = DataStorage.GetMonsterInfo(monsterID);
            if (monster is null)
            {
                var innerErrorCode = await _gameDb.RollbackDelCatchAsync(rollbackID, monsterID, rollbackDate);
                if (innerErrorCode != ErrorCode.None)
                {
                    _logger.LogError($"{nameof(RemoveCatchPost)} ErrorCode : {innerErrorCode}");
                }

                return new Tuple<ErrorCode, Monster>(ErrorCode.RemoveCatchFailNoMonster, null);
            }
            return new Tuple<ErrorCode, Monster>(ErrorCode.None, monster);
        }
    }
}