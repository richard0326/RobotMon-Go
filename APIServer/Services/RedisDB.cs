using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CloudStructures;
using CloudStructures.Structures;

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

        // TODO 
        public static async Task SetUserAuthToken(string key, string authToken)
        {
            var redis = new RedisString<string>(s_connection, key, null);
            await redis.SetAsync(authToken, null);
        }

        public static async Task<string> GetUserAuthToken(string key)
        {
            var redis = new RedisString<string>(s_connection, key, null);

            try
            {
                var redisResult = await redis.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }
                
                var authToken = redisResult.Value;
                return authToken;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}