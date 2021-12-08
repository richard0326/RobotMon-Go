using System;
using ServerCommon;

namespace ApiServer.Model
{
    public class UserInfoResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        // TODO UID도 유저에게 주면 좋을 것 같긴 합니다...
        
        //public List<Monster> MonsterList { get; set; } // 유저가 소유 중인 로봇 몬스터 정보
        //public List<Friend> FriendList { get; set; } // 유저와 친구인 유저 목록
        //public List<MonsterCandy> MonsterCandyList {get; set; } // 몬스터 강화에 사용되는 캔디 목록
        public Int32 StarPoint { get; set; } // 별의 모래
        public Int32 RankPoint { get; set; }  // 랭킹 점수
    }
}