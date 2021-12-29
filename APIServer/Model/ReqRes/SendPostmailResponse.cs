using ServerCommon;

namespace ApiServer.Model
{
    public class SendPostmailResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}