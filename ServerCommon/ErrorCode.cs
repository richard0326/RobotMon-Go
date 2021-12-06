namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        // API Server 전용  20201 ~ 20400
        Login_Fail_Exception = 20201,
        Login_Fail_NoUserExist = 20202,
        Login_Fail_WrongPassword = 20203,
        
        CreateAccount_Fail_Exception = 20211,
        CreateAccount_Fail_Duplicate = 20212,

        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
        
        // DB Error 20801 ~ 21000
        DbConnection_Fail = 20801,
    }
}