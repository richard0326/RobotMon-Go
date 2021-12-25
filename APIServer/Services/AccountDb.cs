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
        
        public async Task<Tuple<ErrorCode, Int64>> CreateAccountDataAsync(string id, string pw, string salt)
        {
            try
            {
                var insertQuery = $"insert Users(ID, PW, Salt) Values(@userId, @userPw, @userSalt); SELECT LAST_INSERT_ID();";
                var lastInsertId = await _dbConn.QueryFirstAsync<Int32>(insertQuery, new
                {
                    userId = id,
                    userPw = pw,
                    userSalt = salt
                });

                return new Tuple<ErrorCode, long>(ErrorCode.None, lastInsertId);
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(CreateAccountDataAsync)} Exception : {e}");
                return new Tuple<ErrorCode, long>(ErrorCode.CreateAccountFailDuplicate, 0);
            }
        }

        public async Task<ErrorCode> RollbackCreateAccountDataAsync(Int64 createIdx)
        {
            try
            {
                var deleteQuery = $"delete from Users where UID = {createIdx}";
                var count = await _dbConn.ExecuteAsync(deleteQuery);
                
                if (count == 0)
                {
                    _logger.ZLogDebug($"{nameof(RollbackCreateAccountDataAsync)} Error : {ErrorCode.RollbackCreateAccountFailDeleteQuery}");
                    return ErrorCode.RollbackCreateAccountFailDeleteQuery;
                }
                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(RollbackCreateAccountDataAsync)} Exception : {e}");
                return ErrorCode.RollbackSendCreateAccountFailException;
            }
        }
        
        public async Task<ErrorCode> TryPasswordAsync(string id, string pw)
        {
            try
            {
                var selectQuery = $"select PW, Salt from Users where ID = @userId";
                var loginData = await _dbConn.QuerySingleOrDefaultAsync<TableLoginData>(selectQuery, new
                {
                    userId = id
                });
                
                if (loginData is null)
                {
                    return ErrorCode.LoginFailNoUserExist;
                }

                if (string.IsNullOrWhiteSpace(loginData.PW))
                {
                    return ErrorCode.LoginFailNoUserExist;
                }
                
                // password 일치 여부 확인
                var hashingPassword = Security.MakeHashingPassWord(loginData.Salt, pw);
                if (loginData.PW != hashingPassword)
                {
                    return ErrorCode.LoginFailWrongPassword;
                }
            }
            catch (Exception e)
            {
                _logger.ZLogDebug($"{nameof(TryPasswordAsync)} Exception : {e}");
                return ErrorCode.LoginFailException;
            }

            return ErrorCode.None;
        }
    }
}