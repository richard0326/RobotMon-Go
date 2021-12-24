using ApiServer.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ServerCommon;
using ZLogger;

namespace ApiServer.Services
{
    public class RankManager
    {
        public static async void Init(string dbConnString)
        {
            // 서버 실행 시 초기 모든 유저 정보를 읽어서 Redis Rank에 올려둔다.
            using var dBConn = new MySqlConnection(dbConnString);
            {
                dBConn.Open();

                try
                {
                    var gameInfoList = dBConn.Query<TableUserGameInfo>("select * from gamedata");
                    foreach (var gameinfo in gameInfoList)
                    {
                        if (await RedisDB.ZSetAddAsync(gameinfo.ID, gameinfo.StarPoint) == false)
                        {
                            Console.WriteLine($"Init fail {gameinfo.ID} {gameinfo.StarPoint}");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{nameof(RankManager)} {nameof(Init)} exception: {e}");
                    throw;
                }
                
            }
        }

        public static async Task<ErrorCode> UpdateStarCount(string id, Int32 starCount, IGameDb gameDb)
        {
            // Redis의 랭킹 값을 변경을 시도한다.
            if (await RedisDB.ZSetIncreamentRankAsync(id, starCount) == false)
            {
                // logger
                return ErrorCode.RankManagerFailUpdateStarCountIncrease;
            }
            
            // Db의 유저 정보를 변경을 시도한다.
            var result = await gameDb.UpdateUserStarCountAsync(id, starCount);
            if (result != ErrorCode.None)
            {
                // rollback
                if (await RedisDB.ZSetIncreamentRankAsync(id, -starCount) == false)
                {
                    //logger.ZLogDebug($"{nameof(UpdateStarCount)} Rank Rollback Fail!!!");
                    return ErrorCode.RankManagerFailUpdateStarCountNeedRollback;
                }
                return ErrorCode.RankManagerFailUpdateStarCountDbFail;
            }
            
            return ErrorCode.None;
        }
        
    }
}