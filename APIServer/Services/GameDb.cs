using System;
using System.Data;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Options;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerCommon;
using ZLogger;

namespace ApiServer.Services
{
    public class GameDb : IGameDb
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection _dbConn;
        private readonly ILogger<GameDb> _logger;

        public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            _logger = logger;
            Open();
            //_logger.ZLogDebug($"Open");
        }

        public void Dispose()
        {
            Close();
            //_logger.ZLogDebug($"Dispose");
        }
        
        public void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.GameConnStr);

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn.Close();
        }
        
        // 게임 정보 가져오기
        public async Task<TableUserGameInfo> GetUserGameInfoAsync(string id)
        {
            // UID로 검색하면 더 좋을 듯.
            var SelectQuery = $"select StarPoint, RankPoint, UserLevel, UserExp from userdata where ID = @userId";
            
            try
            {
                var gameData = await _dbConn.QuerySingleOrDefaultAsync<TableUserGameInfo>(SelectQuery, new
                {
                    userId = id
                });
                
                if (gameData == null)
                {
                    return null;
                }

                return gameData;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"GetLoginData_Exception : {e}");
                return null;
            }
        }
        
        // 게임 정보 설정하기
        public async Task<ErrorCode> SetUserGameInfoAsync(TableUserGameInfo gameInfo)
        {
            var InsertQuery = $"insert userdata(ID, StarPoint, RankPoint, UserLevel, UserExp) Values(@userId, {gameInfo.StarPoint}, {gameInfo.RankPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp})";

            try
            {
                var count = await _dbConn.ExecuteAsync(InsertQuery, new
                {
                    userId = gameInfo.ID
                });

                // ID를 Unique하게 해놔서... 일로 들어오진 않는다.
                if (count != 1)
                {
                    return ErrorCode.UserGameInfoFailDuplicate;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"CreateAccount_Exception : {e}");
                return ErrorCode.UserGameInfoFailDuplicate;
            }
            return ErrorCode.None;
        }
    }
}