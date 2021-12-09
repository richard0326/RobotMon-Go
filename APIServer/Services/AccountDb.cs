using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Options;
using CloudStructures;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerCommon;
using ZLogger;

namespace ApiServer.Services
{
    public class AccountDb : IAccountDb
    {
        private readonly IOptions<DbConfig> _dbConfig;
        private IDbConnection _dbConn;
        private readonly ILogger<AccountDb> _logger;

        public AccountDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            _logger = logger;
            Open();
            //_logger.ZLogDebug($"Open");
        }

        public void Dispose()
        {
            Close();
            //_logger.ZLogDebug($"Dispose");
        }
        
        public void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.AccountConnStr);

            _dbConn.Open();
        }
        
        public void Close()
        {
            _dbConn.Close();
        }
        
        public async Task<ErrorCode> CreateAccountDataAsync(string id, string pw, string salt)
        {
            var InsertQuery = $"insert Users(ID, PW, Salt) Values(@userId, @userPw, @userSalt)";
            
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
                    return ErrorCode.CreateAccountFailDuplicate;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"CreateAccount_Exception : {e}");
                return ErrorCode.CreateAccountFailDuplicate;
            }

            return ErrorCode.None;
        }
        
        // 유저의 Password, Salt 값 반환
        public async Task<Tuple<string, string>> GetPasswordInfoAsync(string id, string pw)
        {
            var SelectQuery = $"select PW, Salt from Users where ID = @userId";
            
            try
            {
                var loginData = await _dbConn.QuerySingleOrDefaultAsync<TableLoginData>(SelectQuery, new
                {
                    userId = id
                });
                
                if (loginData == null)
                {
                    return null;
                }
                
                return new Tuple<string, string>(loginData.PW, loginData.Salt);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"GetLoginData_Exception : {e}");
                return null;
            }
        }
    }
}