namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        // API Server 전용  20201 ~ 20400
        LoginFailUserAlreadyExist = 20201,
        LoginFailNoUserExist = 20202,
        LoginFailWrongPassword = 20203,
        LoginFailRedisError = 20204,
        
        CreateAccountFailDBFail = 20211,
        CreateAccountFailDuplicate = 20212,

        UserInfoFailLoginFail = 20221,
        UserInfoFailWrongToken = 20222,
        
        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
        
        // DB Error 20801 ~ 21000
    }
}