﻿using ServerCommon;

namespace ApiServer.Model
{
    public class DailyCheckResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}