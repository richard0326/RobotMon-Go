using ServerCommon;

namespace ApiServer.Model
{
    public class CheckPostmailResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 TotalSize { get; set; }
        public List<Tuple<Int64,Int32>> PostmailInfo { get; set; }
    }
}