using ServerCommon;

namespace ApiServer.Model
{
    public class CheckCatchResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public List<Tuple<Int64, Int64, DateTime>> MonsterInfoList { get; set; } =
            new List<Tuple<long, long, DateTime>>();
    }
}