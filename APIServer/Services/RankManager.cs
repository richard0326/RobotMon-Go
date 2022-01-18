﻿using ApiServer.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ServerCommon;


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
                            Console.WriteLine($"{nameof(RankManager)} {nameof(Init)} Error : Init fail {gameinfo.ID} {gameinfo.StarPoint}");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{nameof(RankManager)} {nameof(Init)} Exception: {e}");
                    throw;
                }
                
            }
        }

        public static async Task<ErrorCode> UpdateStarCount(string id, Int32 starCount, IGameDb gameDb)
        {
            // Redis의 랭킹 값을 변경을 시도한다.
            if (await RedisDB.UpdateRankAsync(id, starCount) == false)
            {
                return ErrorCode.RankManagerFailUpdateStarCountIncrease;
            }
            
            // Db의 유저 정보를 변경을 시도한다.
            var result = await gameDb.UpdateUserStarCountAsync(id, starCount);
            if (result != ErrorCode.None)
            {
                // rollback
                if (await RedisDB.UpdateRankAsync(id, -starCount) == false)
                {
                    return ErrorCode.RankManagerFailUpdateStarCountNeedRollback;
                }
                return ErrorCode.RankManagerFailUpdateStarCountDbFail;
            }
            
            return ErrorCode.None;
        }

        public static async Task<ErrorCode> RollbackUpdateStarCount(string id, Int32 minusStarCount, IGameDb gameDb)
        {
            // Redis의 랭킹 값을 변경을 시도한다.
            if (await RedisDB.UpdateRankAsync(id, -minusStarCount) == false)
            {
                return ErrorCode.RollbackRankManagerFailUpdateStarCountIncrease;
            }
            
            // Db의 유저 정보를 변경을 시도한다.
            var result = await gameDb.UpdateUserStarCountAsync(id, -minusStarCount);
            if (result != ErrorCode.None)
            {
                // rollback
                if (await RedisDB.UpdateRankAsync(id, minusStarCount) == false)
                {
                    return ErrorCode.RollbackRankManagerFailUpdateStarCountNeedRollback;
                }
                return ErrorCode.RollbackRankManagerFailUpdateStarCountDbFail;
            }
            
            return ErrorCode.None;
        }

        public static async Task<Tuple<ErrorCode, Int64, string[]>> CheckRankingInfo(Int32 pageIndex, Int32 range = 10)
        {
            var rankList = await RedisDB.GetRangeRankAsync(pageIndex * range, range);
            if (rankList is null)
            {
                return new Tuple<ErrorCode, Int64, string[]>(ErrorCode.RankManagerFailGetRangeRank, 0, null);
            }

            var count = await RedisDB.GetRankSizeAsync();
            if (count == -1)
            {
                return new Tuple<ErrorCode, Int64, string[]>(ErrorCode.RankManagerFailGetRankSize, 0, null);
            }

            return new Tuple<ErrorCode, Int64, string[]>(ErrorCode.None, count, rankList);
        }
    }
}