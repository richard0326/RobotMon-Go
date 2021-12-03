﻿using System.Threading.Tasks;
using CloudStructures;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace ApiServer
{
    public class MemoryManager
    {
        static string s_gameDBConnectString;
        static string s_redisAddress;
        
        public static RedisConnection s_redisConnection { get; set; }
        
        public static void Init(IConfiguration configuration)
        {
            // Conf 파일에서 DB와 Redis 정보를 긁어온다.
            s_gameDBConnectString = configuration.GetSection("DBConnection")["MySqlGame"];
            s_redisAddress = configuration.GetSection("DBConnection")["Redis"];
            
            var config = new RedisConfig("basic", s_redisAddress);
            s_redisConnection = new RedisConnection(config);
        }
        
        public static async Task<MySqlConnection> GetGameDBConnection()
        {
            // GetOpenMySqlConnection함수 통해서 DB 연결하기...
            return await GetOpenMySqlConnection(s_gameDBConnectString);
        }
        
        static async Task<MySqlConnection> GetOpenMySqlConnection(string connectionString)
        {
            // 실질적임 함수 구현
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}