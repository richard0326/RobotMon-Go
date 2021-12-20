using ServerCommon;

namespace ApiServer.Model
{
    public class RecvPostmailResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 StarCount { get; set; }
    }
}