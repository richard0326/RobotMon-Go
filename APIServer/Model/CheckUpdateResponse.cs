using ServerCommon;

namespace ApiServer.Model
{
    public class CheckUpdateResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 UpgradeCost { get; set; }
        public Int32 StarCost { get; set; }
    }
}