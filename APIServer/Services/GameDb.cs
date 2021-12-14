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
        private IDbTransaction _dBTransaction;
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
            var SelectQuery = $"select StarPoint, RankPoint, UserLevel, UserExp from gamedata where ID = @userId";
            
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
                _logger.ZLogDebug($"{nameof(GetUserGameInfoAsync)} Exception : {e}");
                return null;
            }
        }
        
        // 게임 정보 설정하기
        public async Task<ErrorCode> SetUserGameInfoAsync(TableUserGameInfo gameInfo)
        {
            var InsertQuery = $"insert gamedata(ID, StarPoint, RankPoint, UserLevel, UserExp) Values(@userId, {gameInfo.StarPoint}, {gameInfo.RankPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp})";

            try
            {
                var count = await _dbConn.ExecuteAsync(InsertQuery, new
                {
                    userId = gameInfo.ID
                });
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(SetUserGameInfoAsync)} Exception : {e}");
                return ErrorCode.UserGameInfoFailException;
            }
            return ErrorCode.None;
        }

        public async Task<FieldMonsterResponse> GetMonsterInfoAsync(Int64 monsterUID)
        {
            var SelectQuery = $"select MonsterName, Type, Level, HP, Att, Def, StarCount from monsterinfo where MID = {monsterUID}";

            try
            {
                var monsterInfo = await _dbConn.QuerySingleOrDefaultAsync<TableMonsterInfo>(SelectQuery);
                
                if (monsterInfo == null)
                {
                    return null;
                }

                return new FieldMonsterResponse()
                {
                    MonsterID = monsterUID,
                    Name = monsterInfo.MonsterName,
                    Type = monsterInfo.Type,
                    Att = monsterInfo.Att,
                    Def = monsterInfo.Def,
                    HP = monsterInfo.HP,
                    Level = monsterInfo.Level,
                    StarCount = monsterInfo.StarCount,
                };
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(GetMonsterInfoAsync)} Exception : {e}");
                throw;
            }
        }

        public async Task<ErrorCode> SetCatchAsync(TableCatch catchTable)
        {
            var InsertQuery = $"insert catch(UserID, MonsterID, CatchTime) Values(@userId, {catchTable.MonsterID}, @catchTime)";

            try
            {
                var count = await _dbConn.ExecuteAsync(InsertQuery, new
                {
                    userId = catchTable.UserID,
                    catchTime = catchTable.CatchTime.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(SetCatchAsync)} Exception : {e}");
                return ErrorCode.CatchFailException;
            }
            return ErrorCode.None;
        }

        public async Task<ErrorCode> TryDailyCheckAsync(string ID)
        {
            var updateQuery =
                $"UPDATE dailycheck Set RewardDate = RewardDate + 1, RewardCount = RewardCount + 1 WHERE ID = @userId and not RewardDate = CURDATE()";

            try
            {
                var count = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                // TODO 진행 예정...
                if (count == 0)
                {
                    // 변화된 쿼리가 없다면... Insert 해줘야함.
                    var insertQuery = $"INSERT INTO dailycheck(`ID`, `RewardCount`, `RewardDate`) VALUES (@userId, 1, CURDATE());";
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Exception : {e}");
                return ErrorCode.DailyCheckFailException;
            }
            return ErrorCode.None;
        }
    }
}