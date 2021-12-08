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
        private readonly IOptions<DbConfig> _accountDbConfig;
        private IDbConnection? _dbConn;
        private readonly ILogger<GameDb> _logger;

        public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> accountDbConfig)
        {
            _accountDbConfig = accountDbConfig;
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
                _dbConn = new MySqlConnection(_accountDbConfig.Value.ConnStr);
            }

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn?.Close();
        }
        
        // TODO 게임 DB 기능 구현
        public async Task<ErrorCode> CreateAccountDataAsync(string? id, string pw, string salt)
        {
            string InsertQuery = $"insert Users(ID, PW, Salt) Values(@userId, @userPw, @userSalt)";
            Console.WriteLine(InsertQuery);
            try
            {
                var count = await _dbConn.ExecuteAsync(InsertQuery, new
                {
                    userId = id,
                    userPw = pw,
                    userSalt = salt
                });

                // ID를 Unique하게 해놔서... 일로 들어오진 않는다.
                if (count != 1)
                {
                    return ErrorCode.CreateAccount_Fail_Duplicate;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"CreateAccount_Exception : @ex", e);
                return ErrorCode.CreateAccount_Fail_Duplicate;
            }

            return ErrorCode.None;
        }
        
        // 유저의 Password, Salt 값 반환
        public async Task<Tuple<string?, string?>?> GetLoginDataAsync(string? id, string? pw)
        {
            string SelectQuery = $"select PW, Salt from Users where ID = @userid";
            
            try
            {
                var loginData = await _dbConn.QuerySingleOrDefaultAsync<TableLoginData>(SelectQuery, new
                {
                    userid = id
                });
                
                if (loginData == null)
                {
                    return null;
                }
                
                return new Tuple<string?, string?>(loginData.PW, loginData.Salt);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"GetLoginData_Exception : @ex", e);
                return null;
            }
        }
    }
}