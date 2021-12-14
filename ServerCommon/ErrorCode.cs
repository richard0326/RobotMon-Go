namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        
        // 미들웨어 반환 코드 (http Response code : Unassigned) 512 ~ 599
        AuthTokenFailNoBody = 512,
        AuthTokenFailNoUser = 513,
        AuthTokenFailWrongAuthToken = 514,
        AuthTokenFailSetNx = 515,

        // API Server 전용  20201 ~ 20400
        LoginFailException = 20201,
        LoginFailNoUserExist = 20202,
        LoginFailWrongPassword = 20203,
        LoginFailRedisError = 20204,
        
        CreateAccountFailDBFail = 20211,
        CreateAccountFailDuplicate = 20212,
        CreateAccountFailGetTable = 20213,

        UserGameInfoFailDuplicate = 20223,
        
        CatchFail = 20231,
        CatchFailDuplicate = 20232,

        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
        
        
        // 기타
        DataStorageReadMonsterFail = 21001,
    }
}