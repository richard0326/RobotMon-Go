﻿using System;
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
        public Task<ErrorCode> UpdateUserStarCountAsync(string ID, Int32 starCount);
        // 유저 정보 설정하기
        public Task<ErrorCode> InitUserGameInfoAsync(TableUserGameInfo table);
        public Task<FieldMonsterResponse> GetMonsterInfoAsync(Int64 monsterUID);
        public Task<Tuple<ErrorCode, Int32>> SetCatchAsync(TableCatch catchTable);
        public Task<ErrorCode> DelCatchAsync(Int64 catchID);
        // 출석체크 설정하기
        public Task<ErrorCode> InitDailyCheckAsync(string ID);
        public Task<Tuple<ErrorCode, DateTime>> TryDailyCheckAsync(string ID);
        public Task<ErrorCode> RollbackDailyCheckAsync(string id, DateTime prevDate);
        public Task<Tuple<Int32, List<Tuple<Int64,Int32>>?>> CheckPostmailAsync(string ID, Int32 pageIndex, Int32 pageSize = 10);
        public Task<Tuple<ErrorCode, Int64>> SendPostmailAsync(string ID, Int32 starCount);
        public Task<ErrorCode> RollbackSendPostmailAsync(Int64 postmailID);
        public Task<Tuple<ErrorCode, Int32, DateTime>> RecvPostmailAsync(string ID, Int64 postmailID);
        public Task<ErrorCode> RollbackRecvPostmailAsync(string id, Int32 startCount, DateTime date);
    }
}