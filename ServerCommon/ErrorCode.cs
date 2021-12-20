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
        
        CreateAccountFailDuplicate = 20212,
        CreateAccountFailGetTable = 20213,

        UserGameInfoFailException = 20223,
        
        CatchFail = 20231,
        CatchFailException = 20232,
        
        InitDailyCheckFailException = 20240,
        TryDailyCheckFailException = 20241,
        DailyCheckFailAlreadyChecked = 20242,
        DailyCheckFailInsertQuery = 20243,
        DailyCheckFailUpdateQuery = 20244,
        DailyCheckFailNoData = 20245,
        
        CheckPostmailFailNoPostmail = 20251,
        CheckPostmailFailException = 20252,
        
        SendPostmailFailException = 20261,
        SendPostmailFailInsert = 20262,
        SendPostmailFailPostmailFull = 20263,

        RecvPostmailFailException = 20271,
        RecvPostmailFailNoPostmail = 20272,
        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800
        
        
        // 기타
        DataStorageReadMonsterFail = 21001,
    }
}