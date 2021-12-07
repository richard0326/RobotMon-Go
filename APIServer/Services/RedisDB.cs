using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApiServer.Model;
using CloudStructures;
using CloudStructures.Structures;
using StackExchange.Redis;

namespace ApiServer.Services
{
    public class RedisDB
    {
        private static RedisConnection? s_connection;

        public static void Init(string address)
        {
            var config = new RedisConfig("redisDb", address);
            s_connection = new RedisConnection(config);
        }

        public static async Task<bool> CheckUserExist(string? key)
        {
            var redisStr = new RedisString<RedisLoginData>(s_connection, key, null);
            
            // TODO redis Async에서 에러가 발생해서 진행되지 않음.
            bool exists = await redisStr.ExistsAsync();
            return exists;
        }
        
        public static async Task SetUserInfo(string key, RedisLoginData redisLoginData)
        {
            var redisStr = new RedisString<RedisLoginData>(s_connection, key, null);
            await redisStr.SetAsync(redisLoginData, null);
        }

        public static async Task<RedisLoginData> GetUserAuthToken(string key)
        {
            var redisStr = new RedisString<RedisLoginData>(s_connection, key, null);

            try
            {
                var redisResult = await redisStr.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }
                
                var redisLogin = redisResult.Value;
                return redisLogin;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}