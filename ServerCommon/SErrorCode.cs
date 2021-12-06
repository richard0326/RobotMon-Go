namespace ServerCommon
{
    public enum ErrorCode : int
    {
        None = 0,
        // API Server 전용  20201 ~ 20400

        // PVP Server 20401 ~ 20600

        // Raid Server  20601 ~ 20800

        // DB  21501 ~ 21800
        login_Fail_NotUser = 21501,
        login_Fail_PW = 21502,
    }
}