using ServerCommon;

namespace ApiServer.Model
{
    public class CheckEvolveResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 UpgradeCandy { get; set; }
    }
}