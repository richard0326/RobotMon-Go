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
        private IDbConnection? _dbConn;
        private readonly ILogger<GameDb> _logger;

        public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            _logger = logger;
            Open();
            _logger.ZLogDebug($"Open");
        }

        public void Dispose()
        {
            Close();
            _logger.ZLogDebug($"Dispose");
        }
        
        public void Open()
        {
            if(_dbConn == null)
            {
                _dbConn = new MySqlConnection(_dbConfig.Value.GameConnStr);
            }

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn?.Close();
        }
        
        // TODO 게임 DB 기능 구현 진행중
        // 유저 정보 가져오기
        public async Task<TableUserInfo> GetUserInfoAsync(string id)
        {
            // UID로 검색하면 더 좋을 듯.
            string SelectQuery = $"select PW, Salt from Userinfo where ID = @userId";
            return null;
        }
        
        // 유저 정보 설정하기
        public async Task<bool> SetUserInfoAsync(TableUserInfo table)
        {
            string InsertQuery = $"insert Users(ID, PW, Salt) Values(@userId, @userPw, @userSalt)";
            return null;
        }
    }
}