using System;
using System.ComponentModel;
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
        private static RedisConnection s_connection;

        public static void Init(string address)
        {
            var config = new RedisConfig("redisDb", address);
            s_connection = new RedisConnection(config);
        }
        
        public static async Task<bool> SetUserInfo(string key, RedisLoginData redisLoginData)
        {
            var redis = new RedisString<RedisLoginData>(s_connection, key, TimeSpan.FromDays(1));

            try
            {
                await redis.SetAsync(redisLoginData, TimeSpan.FromDays(1));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<RedisLoginData> GetUserInfo(string key)
        {
            var redis = new RedisString<RedisLoginData>(s_connection, key, null);

            try
            {
                var redisResult = await redis.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }

                return redisResult.Value;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> DelUserInfo(string key)
        {
            var redis = new RedisString<RedisLoginData>(s_connection, key, null);

            try
            {
                var redisResult = await redis.DeleteAsync();
                return redisResult;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<RedisLoginData> GetUserAuthToken(string key)
        {
            var redis = new RedisString<RedisLoginData>(s_connection, key, null);

            try
            {
                var redisResult = await redis.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }
            
                var redisLogin = redisResult.Value;
                return redisLogin;
            }
            catch
            {
                return null;
            }
        }
    }
}