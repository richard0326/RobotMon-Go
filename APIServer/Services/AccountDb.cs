﻿using System;
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
                _logger.ZLogDebug($"{nameof(CreateAccountDataAsync)} Exception : {e}");
                return ErrorCode.CreateAccountFailDuplicate;
            }

            return ErrorCode.None;
        }
        
        public async Task<ErrorCode> CheckPasswordAsync(string id, string pw)
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
                _logger.ZLogDebug($"{nameof(CheckPasswordAsync)} Exception : {e}");
                return ErrorCode.LoginFailException;
            }

            return ErrorCode.None;
        }
    }
}