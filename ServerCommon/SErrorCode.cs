namespace ServerCommon
{
    // 20000 ~ 
    public enum SErrorCode : int
    {
        None = 0,

        // API Server 전용  20201 ~ 20400
        gwFailInitSetup = 20201,
        gwFailSuperSocketStart = 20202,

        // PVP Server 20401 ~ 20600
        EnterLobby_InvalidLobbyNumber = 20421,
        EnterLobby_FailAdduser = 20422,

        LeaveLobby_InvalidLobbyNumber = 20426,
        LeaveLobby_InvalidUser = 20427,

        // Raid Server  20601 ~ 20800

        // DB  21501 ~ 21800
        login_Fail_NotUser = 21501,
        login_Fail_PW = 21502,
    }
}