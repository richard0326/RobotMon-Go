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

            try
            {
                bool exists = await redisStr.ExistsAsync();
                return exists;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
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