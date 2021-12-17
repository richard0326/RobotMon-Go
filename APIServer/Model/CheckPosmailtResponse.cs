using ServerCommon;

namespace ApiServer.Model
{
    public class CheckPostmailResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public List<Tuple<Int64,Int32>> PostmailInfo { get; set; }
    }
}