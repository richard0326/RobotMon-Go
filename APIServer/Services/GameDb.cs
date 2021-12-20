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
            try
            {
                var selectQuery = $"select StarPoint, RankPoint, UserLevel, UserExp from gamedata where ID = @userId";
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
        public async Task<ErrorCode> InitUserGameInfoAsync(TableUserGameInfo gameInfo)
        {
            try
            {
                var insertQuery = $"insert gamedata(ID, StarPoint, RankPoint, UserLevel, UserExp) " +
                                  $"Values(@userId, {gameInfo.StarPoint}, {gameInfo.RankPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp})";
                var count = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = gameInfo.ID
                });
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(InitUserGameInfoAsync)} Exception : {e}");
                return ErrorCode.UserGameInfoFailException;
            }
            return ErrorCode.None;
        }

        public async Task<FieldMonsterResponse> GetMonsterInfoAsync(Int64 monsterUID)
        {
            try
            {
                var selectQuery = $"select MonsterName, Type, Level, HP, Att, Def, StarCount from monsterinfo where MID = {monsterUID}";
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
                return null;
            }
        }

        public async Task<ErrorCode> SetCatchAsync(TableCatch catchTable)
        {
            try
            {
                var insertQuery = $"insert catch(UserID, MonsterID, CatchTime) Values(@userId, {catchTable.MonsterID}, @catchTime)";
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

        public async Task<ErrorCode> InitDailyCheckAsync(string ID)
        {
            try
            {
                var insertQuery = $"INSERT INTO dailycheck(`ID`, `RewardCount`, `RewardDate`) VALUES (@userId, 1, @dateTime)";
                // 초기 값을 생성해준다.
                var count = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = ID,
                    dateTime = "0000-00-00"
                });

                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(InitDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailInsertQuery}");
                    return ErrorCode.DailyCheckFailInsertQuery;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(InitDailyCheckAsync)} Exception : {e}");
                return ErrorCode.InitDailyCheckFailException;
            }

            return ErrorCode.None;
        }
        
        public async Task<ErrorCode> TryDailyCheckAsync(string ID)
        {
            try
            {
                var selectQuery = $"select RewardCount, RewardDate from dailycheck where ID = @userId";
                // 먼저 select해서 RewardCount와 RewardDate를 가져온다.
                var dailyCheck = await _dbConn.QuerySingleOrDefaultAsync<TableDailyCheck>(selectQuery, new
                {
                    userId = ID
                });
                
                // 회원 가입할때 생성해줄 것이기 때문에.. 값이 항상 있어야함.
                if (dailyCheck is null)
                {
                    _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailNoData}");
                    return ErrorCode.DailyCheckFailNoData;
                }

                // 오늘 이미 받았기 때문에 보상을 받을 수 없다.
                if (dailyCheck.RewardDate == DateTime.Today)
                {
                    return ErrorCode.DailyCheckFailAlreadyChecked;
                }
                
                // data 갱신
                var updateQuery = $"UPDATE dailycheck Set RewardCount = RewardCount + 1, RewardDate = CURDATE() WHERE ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                if (updateCount == 0)
                {
                    _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Error : {ErrorCode.DailyCheckFailUpdateQuery}");
                    return ErrorCode.DailyCheckFailUpdateQuery;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Exception : {e}");
                return ErrorCode.TryDailyCheckFailException;
            }
            return ErrorCode.None;
        }

        public async Task<Tuple<Int32, List<Tuple<Int64,Int32>>?>> CheckPostmailAsync(string ID, Int32 pageIndex, Int32 pageSize)
        {
            try
            {
                var selectQuery = $"select postID, StarCount from postmail where ID = @userId LIMIT {pageIndex * pageSize}, {pageSize}";
                var postmail = await _dbConn.QueryAsync<TablePostmail>(selectQuery, new
                {
                    userId = ID
                });

                // 우편함에 값이 없는 상태임.
                if (postmail is null)
                {
                    return new Tuple<int, List<Tuple<long, int>>?>(0, null);
                }

                // 전체 개수 찾아오기
                var countQuery = $"select count(*) from postmail where ID = @userId";
                var count = await _dbConn.QueryFirstAsync<Int32>(countQuery, new
                {
                    userId = ID
                });
                
                // 반환할 Tuple
                var ret = new Tuple<int, List<Tuple<long, int>>?>(count, new List<Tuple<long, int>>());
                
                // 쿼리를 돌면서 선물함의 ID와 선물 내용을 보내준다.
                foreach (var eachPost in postmail)
                {
                    ret.Item2.Add(new Tuple<Int64, Int32>(eachPost.postID, eachPost.StarCount));
                }

                return ret;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(CheckPostmailAsync)} Exception : {e}");
                return null;
            }
        }

        public async Task<ErrorCode> SendPostmailAsync(string ID, Int32 starCount)
        {
            try
            {
                var insertQuery = $"insert into postmail(ID, StarCount, Date) values (@userID, {starCount}, CURDATE())";
                var insertCount = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = ID
                });

                if (insertCount == 0)
                {
                    // 선물을 넣는데 실패한 경우...
                    return ErrorCode.SendPostmailFailInsert;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(SendPostmailAsync)} Exception : {e}");
                return ErrorCode.SendPostmailFailException;
            }
            
            return ErrorCode.None;
        }
        
        public async Task<Tuple<ErrorCode, Int32>> RecvPostmailAsync(string ID, Int64 postmailID)
        {
            try
            {
                var selectQuery = $"select StarCount from postmail where postID = {postmailID} and ID = @userId";
                var selInfo = await _dbConn.QuerySingleAsync<TablePostmail>(selectQuery, new
                {
                    userId = ID
                });

                if (selInfo is null)
                {
                    // 선물이 없다면 문제가 있는 상황
                    return new Tuple<ErrorCode, Int32>(ErrorCode.RecvPostmailFailNoPostmail, 0);
                }
                
                var delQuery = $"delete from postmail where postID = {postmailID} and ID = @userId";
                var delCount = await _dbConn.ExecuteAsync(delQuery, new
                {
                    userId = ID
                });

                return new Tuple<ErrorCode, Int32>(ErrorCode.None, selInfo.StarCount);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RecvPostmailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int32>(ErrorCode.RecvPostmailFailException, 0);
            }
        }
    }
}