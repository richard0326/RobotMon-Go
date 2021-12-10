using ApiServer.Services;
using ServerCommon;

namespace ApiServer.Model
{
        public class FieldMonsterResponse
        {
                public ErrorCode Result { get; set; } = ErrorCode.None;
                public Monster MonsterObj;
        }
}