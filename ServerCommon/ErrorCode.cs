namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        // API Server 전용  20201 ~ 20400
        Login_Fail_UserAlreadyExist = 20201,
        Login_Fail_NoUserExist = 20202,
        Login_Fail_WrongPassword = 20203,
        Login_Fail_RedisError = 20204,
        
        CreateAccount_Fail_Duplicate = 20212,

        UserInfo_Fail_LoginFail = 20220,
        
        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
        
        // DB Error 20801 ~ 21000
        DbConnection_Fail = 20801,
    }
}