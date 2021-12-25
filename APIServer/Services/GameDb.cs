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
                var selectQuery = $"select StarPoint, UserLevel, UserExp from gamedata where ID = @userId";
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
        public async Task<Tuple<ErrorCode, Int64>> InitUserGameInfoAsync(TableUserGameInfo gameInfo)
        {
            try
            {
                var insertQuery = "insert gamedata(ID, StarPoint, UserLevel, UserExp) " +
                                  $"Values(@userId, {gameInfo.StarPoint}, {gameInfo.UserLevel}, {gameInfo.UserExp}); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = gameInfo.ID
                });

                return new Tuple<ErrorCode, long>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(InitUserGameInfoAsync)} Exception : {e}");
                return new Tuple<ErrorCode, long>(ErrorCode.UserGameInfoFailInitException, 0);
            }
        }

        public async Task<ErrorCode> RollbackInitUserGameInfoAsync(Int64 gamedataId)
        {
            try
            {
                var deleteQuery = $"delete from gamedata where UID = {gamedataId}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackInitUserGameInfoAsync)} Error : {ErrorCode.RollbackInitUserGameIfnoFailDeleteQuery}");
                    return ErrorCode.RollbackInitUserGameIfnoFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RollbackInitUserGameInfoAsync)} Exception : {e}");
                return ErrorCode.RollbackInitUserGameInfoFailException;
            }
        }

        public async Task<ErrorCode> UpdateUserStarCountAsync(string ID, Int32 starCount)
        {
            try
            {
                var updateQuery =
                    $"update gamedata set StarPoint = StarPoint + {starCount} where ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = ID
                });

                if (updateCount == 0)
                {
                    _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Error : {ErrorCode.UserGameInfoFailStarCountUpdateFail}");
                    return ErrorCode.UserGameInfoFailStarCountUpdateFail;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(InitUserGameInfoAsync)} Exception : {e}");
                return ErrorCode.UserGameInfoFailStarCountException;
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

        public async Task<Tuple<ErrorCode, Int32>> SetCatchAsync(TableCatch catchTable)
        {
            try
            {
                var insertQuery = $"insert catch(UserID, MonsterID, CatchTime) Values(@userId, {catchTable.MonsterID}, @catchTime); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = catchTable.UserID,
                    catchTime = catchTable.CatchTime.ToString("yyyy-MM-dd")
                });
                
                return new Tuple<ErrorCode, int>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(SetCatchAsync)} Exception : {e}");
                return new Tuple<ErrorCode, int>(ErrorCode.CatchFailException, 0);
            }
        }

        public async Task<ErrorCode> DelCatchAsync(Int64 catchID)
        {
            try
            {
                var deleteQuery = $"delete from catch where CatchID = {catchID}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(DelCatchAsync)} Error : {ErrorCode.CatchFailDeleteFail}");
                    return ErrorCode.CatchFailDeleteFail;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(DelCatchAsync)} Exception : {e}");
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
        
        public async Task<ErrorCode> RollbackInitDailyCheckAsync(string dailyID)
        {
            try
            {
                var deleteQuery = $"delete from dailycheck where ID = @userId";
                var count = await _dbConn.ExecuteAsync(deleteQuery, new
                {
                    userId = dailyID
                });
                
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackSendPostmailAsync)} Error : {ErrorCode.RollbackInitDailyCheckFailDeleteQuery}");
                    return ErrorCode.RollbackInitDailyCheckFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RollbackSendPostmailAsync)} Exception : {e}");
                return ErrorCode.RollbackInitDailyCheckFailException;
            }
        }
        
        public async Task<Tuple<ErrorCode, DateTime>> TryDailyCheckAsync(string ID)
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
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailNoData, new DateTime());
                }

                // 오늘 이미 받았기 때문에 보상을 받을 수 없다.
                if (dailyCheck.RewardDate == DateTime.Today)
                {
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailAlreadyChecked, new DateTime());
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
                    return new Tuple<ErrorCode, DateTime>(ErrorCode.DailyCheckFailUpdateQuery, new DateTime());
                }
                
                return new Tuple<ErrorCode, DateTime>(ErrorCode.None, dailyCheck.RewardDate);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(TryDailyCheckAsync)} Exception : {e}");
                return new Tuple<ErrorCode, DateTime>(ErrorCode.TryDailyCheckFailException, new DateTime());
            }
        }

        public async Task<ErrorCode> RollbackDailyCheckAsync(string id, DateTime prevDate)
        {
            try
            {
                var updateQuery = $"UPDATE dailycheck Set RewardCount = RewardCount - 1, RewardDate = @date WHERE ID = @userId";
                var updateCount = await _dbConn.ExecuteAsync(updateQuery, new
                {
                    userId = id,
                    date = prevDate.ToString("yy-MM-dd")
                });
                
                if (updateCount == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackDailyCheckAsync)} Error : {ErrorCode.RollbackDailyCheckFailUpdateQuery}");
                    return ErrorCode.RollbackDailyCheckFailUpdateQuery;
                }
                
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RollbackDailyCheckAsync)} Exception : {e}");
                return ErrorCode.RollbackDailyCheckFailException;
            }
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

        public async Task<Tuple<ErrorCode, Int64>> SendPostmailAsync(string ID, Int32 starCount)
        {
            try
            {
                var insertQuery = $"insert into postmail(ID, StarCount, Date) values (@userID, {starCount}, CURDATE()); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = ID
                });

                return new Tuple<ErrorCode, Int64>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(SendPostmailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int64>(ErrorCode.SendPostmailFailException, 0);
            }
        }

        public async Task<ErrorCode> RollbackSendPostmailAsync(Int64 postmailID)
        {
            try
            {
                var deleteQuery = $"delete from postmail where postID = {postmailID}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackSendPostmailAsync)} Error : {ErrorCode.RollbackSendPostmailFailDeleteQuery}");
                    return ErrorCode.RollbackSendPostmailFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RollbackSendPostmailAsync)} Exception : {e}");
                return ErrorCode.RollbackSendPostmailFailException;
            }
        }
        
        public async Task<Tuple<ErrorCode, Int32, DateTime>> RecvPostmailAsync(string ID, Int64 postmailID)
        {
            try
            {
                var selectQuery = $"select ID, StarCount, Date from postmail where postID = {postmailID} and ID = @userId";
                var selInfo = await _dbConn.QuerySingleAsync<TablePostmail>(selectQuery, new
                {
                    userId = ID
                });

                if (selInfo is null)
                {
                    // 선물이 없다면 문제가 있는 상황
                    return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.RecvPostmailFailNoPostmail, 0, new DateTime());
                }
                
                var delQuery = $"delete from postmail where postID = {postmailID} and ID = @userId";
                var delCount = await _dbConn.ExecuteAsync(delQuery, new
                {
                    userId = ID
                });

                return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.None, selInfo.StarCount, selInfo.Date);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RecvPostmailAsync)} Exception : {e}");
                return new Tuple<ErrorCode, Int32, DateTime>(ErrorCode.RecvPostmailFailException, 0, new DateTime());
            }
        }

        public async Task<ErrorCode> RollbackRecvPostmailAsync(string id, Int32 startCount, DateTime date)
        {
            try
            {
                var insertQuery = $"insert into postmail(ID, StarCount, Date) values (@userId, {startCount}, @dateStr)";
                var count = await _dbConn.ExecuteAsync(insertQuery, new
                {
                    userId = id,
                    dateTime = date.ToString("yy-MM-dd")
                });
                
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackRecvPostmailAsync)} Error : {ErrorCode.RollbackRecvPostmailFailInsertQuery}");
                    return ErrorCode.RollbackRecvPostmailFailInsertQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RecvPostmailAsync)} Exception : {e}");
                return ErrorCode.RollbackRecvPostmailFailException;
            }
        }
    }
}