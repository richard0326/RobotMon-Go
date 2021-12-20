using System;
using System.Threading.Tasks;
using ApiServer.Model;
using ServerCommon;

namespace ApiServer.Services
{
    public interface IGameDb : IDisposable
    {
        // DB 열기.
        public void Open();
        
        // DB 닫기.
        public void Close();

        // 유저 정보 가져오기
        public Task<TableUserGameInfo> GetUserGameInfoAsync(string id);
        
        // 유저 정보 설정하기
        public Task<ErrorCode> InitUserGameInfoAsync(TableUserGameInfo table);

        public Task<FieldMonsterResponse> GetMonsterInfoAsync(Int64 monsterUID);
        
        public Task<ErrorCode> SetCatchAsync(TableCatch catchTable);

        // 출석체크 설정하기
        public Task<ErrorCode> InitDailyCheckAsync(string ID);
        public Task<ErrorCode> TryDailyCheckAsync(string ID);
        public Task<Tuple<Int32, List<Tuple<Int64,Int32>>?>> CheckPostmailAsync(string ID, Int32 pageIndex, Int32 pageSize = 10);
        public Task<ErrorCode> SendPostmailAsync(string ID, Int32 starCount);
        public Task<Tuple<ErrorCode, Int32>> RecvPostmailAsync(string ID, Int64 postmailID);
    }
}