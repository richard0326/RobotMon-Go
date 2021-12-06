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
    public class CommonDb : ICommonDb
    {
        private readonly IOptions<CommonDbConfig> HandleCommonDbConfig;
        private IDbConnection DBConn;
        
        public CommonDb(IOptions<CommonDbConfig> commonDbConfig)
        {
            HandleCommonDbConfig = commonDbConfig;
            //Open();
        }

        //public void Dispose()
        //{
        //    Close();
        //}
        
        public void Open()
        {
            if(DBConn == null)
            {
                //DBConn = new MySqlConnection(HandleCommonDbConfig.Value.ConnStr);
                DBConn = new MySqlConnection("server=127.0.0.1;user=root;password=root1234;port=3306;database=game;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;");
            }

            DBConn.Open();
        }
        
        public void Close()
        {
            DBConn?.Close();
        }
        
        public async Task<ServerCommon.ErrorCode> InsertCreateAccountDataAsync(string id, string pw, string salt)
        {
            try
            {
                Open();
            }
            catch (Exception e)
            {
                return ServerCommon.ErrorCode.DbConnection_Fail;
            }

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
                
                Close();
            }
            catch (Exception e)
            {
                Close();
                return ServerCommon.ErrorCode.CreateAccount_Fail_Exception;
            }
            
            return ServerCommon.ErrorCode.None;
        }
        
        public async Task<ServerCommon.ErrorCode> GetLoginDataAsync(string id, string pw)
        {
            try
            {
                Open();
            }
            catch (Exception e)
            {
                return ServerCommon.ErrorCode.DbConnection_Fail;
            }

            string SelectQuery = $"select PW, Salt from Users where ID = @userid";
            
            try
            {
                var loginData = await DBConn.QuerySingleOrDefaultAsync<TbLoginData>(SelectQuery, new
                {
                    userid = id
                });
                if (loginData == null || string.IsNullOrWhiteSpace(loginData.PW))
                {
                    return ServerCommon.ErrorCode.Login_Fail_NoUserExist;
                }

                var hashingPassword = ServerCommon.Security.MakeHashingPassWord(loginData.Salt, pw);

                if (loginData.PW != hashingPassword)
                {
                    return ServerCommon.ErrorCode.Login_Fail_WrongPassword;
                }
                Close();
            }
            catch (Exception e)
            {
                Close();
                return ServerCommon.ErrorCode.Login_Fail_Exception;
            }
            
            return ServerCommon.ErrorCode.None;
        }
    }
}