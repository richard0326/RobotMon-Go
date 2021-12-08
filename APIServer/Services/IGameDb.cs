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
        public Task<TableUserInfo> GetUserInfoAsync(string id);
        
        // 유저 정보 설정하기
        public Task<bool> SetUserInfoAsync(TableUserInfo table);
    }
}