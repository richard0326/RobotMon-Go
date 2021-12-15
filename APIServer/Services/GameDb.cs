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
            var selectQuery = $"select StarPoint, RankPoint, UserLevel, UserExp from gamedata where ID = @userId";
            
            try
            {
                var gameData = await _dbConn.QuerySingleOrDefaultAsync<TableUserGameInfo>(selectQuery, new
                {
                    userId = id
                });
                
                if(gameData is null)
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
            var insertQuery = $"insert gamedata(ID, StarPoint, RankPoint, UserLevel, UserExp) Values(@userId, {gameInfo.StarPoint}, {gameInfo.RankPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp})";

            try
            {
                var count = await _dbConn.ExecuteAsync(insertQuery, new
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
            var selectQuery = $"select MonsterName, Type, Level, HP, Att, Def, StarCount from monsterinfo where MID = {monsterUID}";

            try
            {
                var monsterInfo = await _dbConn.QuerySingleOrDefaultAsync<TableMonsterInfo>(selectQuery);
                
                if (monsterInfo is null)
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
            var insertQuery = $"insert catch(UserID, MonsterID, CatchTime) Values(@userId, {catchTable.MonsterID}, @catchTime)";

            try
            {
                var count = await _dbConn.ExecuteAsync(insertQuery, new
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

        public async Task<DailyCheckResponse> TryDailyCheckAsync(string ID)
        {
            var response = new DailyCheckResponse();
            var selectQuery = $"select RewardCount, RewardDate from dailycheck where ID = @userId";
            var updateQuery =
                $"UPDATE dailycheck Set RewardCount = RewardCount + 1, RewardDate = CURDATE() WHERE ID = @userId and not RewardDate = CURDATE()";
            var insertQuery = $"INSERT INTO dailycheck(`ID`, `RewardCount`, `RewardDate`) VALUES (@userId, 1, CURDATE());";
            
            try
            {
                // 먼저 select해서 RewardCount와 RewardDate를 가져온다.
                var dailyCheck = await _dbConn.QuerySingleOrDefaultAsync<TableDailyCheck>(selectQuery, new
                {
                    userId = ID
                });
                if (dailyCheck is null)
                {
                    // 초기 값을 생성해준다.
                    var count = await _dbConn.ExecuteAsync(insertQuery, new
                    {
                        userId = ID
                    });

                    if (count == 0)
                    {
                        // 무조건 들어가야하는 상황인데 들어가지 못했다면 문제가 있는 상황임.
                        response.Result = ErrorCode.DailyCheckFailInsertQuery;
                        _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Error : {response.Result}");
                        return response;
                    }

                    // 보상
                    var dailyInfoFirst = DataStorage.GetDailyInfo(1);
                    response.StarCount = dailyInfoFirst.StarCount;
                    
                    // 초기 값 넣기 성공
                    return response;
                }

                // 날짜 일치 확인
                if (dailyCheck.RewardDate == DateTime.Today)
                {
                    // 오늘 이미 받았기 때문에 보상을 받을 수 없다.
                    response.Result = ErrorCode.DailyCheckFailAlreadyChecked;
                    return response;
                }
                
                // 보상 확인
                var dailyInfo = DataStorage.GetDailyInfo(dailyCheck.RewardCount + 1);
                if (dailyInfo is null)
                {
                    // 모든 보상을 다 받은 상태
                    response.Result = ErrorCode.DailyCheckFailMaxReceive;
                    return response;
                }
                
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                if (updateCount == 0)
                {
                    // 무조건 들어가야하는 상황인데 들어가지 못했다면 문제가 있는 상황임.
                    response.Result = ErrorCode.DailyCheckFailUpdateQuery;
                    _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Error : {response.Result}");
                    return response;
                }
                
                response.StarCount = dailyInfo.StarCount;
            }
            catch (Exception e)
            {
                response.Result = ErrorCode.DailyCheckFailException;
                _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Exception : {response.Result}");
                return response;
            }
            return response;
        }
    }
}