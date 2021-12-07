using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ApiServer.Model;
using ApiServer.Options;
using CloudStructures;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerCommon;

namespace ApiServer.Services
{
    public class AccountDb : IAccountDb, IDisposable
    {
        private readonly IOptions<AccountDbConfig> _accountDbConfig;
        private IDbConnection DBConn;
        
        public AccountDb(IOptions<AccountDbConfig> accountDbConfig)
        {
            _accountDbConfig = accountDbConfig;
            Open();
        }

        public void Dispose()
        {
            Close();
        }
        
        public void Open()
        {
            if(DBConn == null)
            {
                DBConn = new MySqlConnection(_accountDbConfig.Value.ConnStr);
            }

            DBConn.Open();
        }
        
        public void Close()
        {
            DBConn?.Close();
        }
        
        public async Task<ServerCommon.ErrorCode> CreateAccountDataAsync(string id, string pw, string salt)
        {
            string InsertQuery = $"insert Users(ID, PW, Salt) Values(@userid, @userpw, @usersalt)";
            Console.WriteLine(InsertQuery);
            try
            {
                var count = await DBConn.ExecuteAsync(InsertQuery, new
                {
                    userid = id,
                    userpw = pw,
                    usersalt = salt
                });

                // ID를 Unique하게 해놔서... 일로 들어오진 않는다.
                if (count != 1)
                {
                    return ErrorCode.CreateAccount_Fail_Duplicate;
                }
            }
            catch (Exception e)
            {
                return ErrorCode.CreateAccount_Fail_Duplicate;
            }

            return ErrorCode.None;
        }
        
        // 유저의 Password, Salt 값 반환
        public async Task<Tuple<string, string>> GetLoginDataAsync(string id, string pw)
        {
            string SelectQuery = $"select PW, Salt from Users where ID = @userid";
            
            try
            {
                var loginData = await DBConn.QuerySingleOrDefaultAsync<TableLoginData>(SelectQuery, new
                {
                    userid = id
                });
                
                if (loginData == null)
                {
                    return null;
                }
                
                return new Tuple<string, string>(loginData.PW, loginData.Salt);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}