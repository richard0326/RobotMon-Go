﻿namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        // API Server 전용  20201 ~ 20400
        LoginFailException = 20201,
        LoginFailNoUserExist = 20202,
        LoginFailWrongPassword = 20203,
        LoginFailRedisError = 20204,
        
        CreateAccountFailDBFail = 20211,
        CreateAccountFailDuplicate = 20212,

        UserGameInfoFailLoginFail = 20221,
        UserGameInfoFailWrongToken = 20222,
        UserGameInfoFailDuplicate = 20223,
        
        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
    }
}