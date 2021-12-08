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
        private static RedisConnection? s_connection = null;

        public static void Init(string address)
        {
            var config = new RedisConfig("redisDb", address);
            s_connection = new RedisConnection(config);
        }

        public static async Task<bool> CheckUserExist(string key)
        {
            if (s_connection != null)
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);

                try
                {
                    var redisResult = await redis.ExistsAsync();
                    return redisResult;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }
        
        public static async Task<bool> SetUserInfo(string key, RedisLoginData redisLoginData)
        {
            if (s_connection != null)
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);

                try
                {
                    await redis.SetAsync(redisLoginData, null);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static async Task<RedisLoginData> GetUserInfo(string? key)
        {
            if (s_connection != null)
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
                catch (Exception e)
                {
                    return null;
                }
            }

            return null;
        }
        
        public static async Task<bool> DelUserInfo(string key)
        {
            if (s_connection != null)
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);

                try
                {
                    var redisResult = await redis.DeleteAsync();
                    return redisResult;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static async Task<RedisLoginData> GetUserAuthToken(string key)
        {
            if (s_connection != null)
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
                catch (Exception e)
                {
                    return null;
                }
            }

            return null;
        }
    }
}