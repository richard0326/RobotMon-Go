using ApiServer.Services;
using ServerCommon;

namespace ApiServer.Model
{
        public class FieldMonsterResponse
        {
                public ErrorCode Result { get; set; } = ErrorCode.None;
                public Int64 MonsterID { get; set; }
                public string Name { get; set; }
                public string Type { get; set; }
                public Int32 Level { get; set; }
                public Int32 HP { get; set; }
                public Int32 Att { get; set; }
                public Int32 Def { get; set; }
                public Int64 StarCount { get; set; }
        }
}