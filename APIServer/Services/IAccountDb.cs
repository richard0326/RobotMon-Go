using System;
using System.Threading.Tasks;

namespace ApiServer
{
    public interface IAccountDb
    {
        // DB 열기.
        public void Open();
        
        // DB 닫기.
        public void Close();

        public Task<ServerCommon.ErrorCode> CreateAccountDataAsync(string id, string pw, string salt);
        
        // 유저의 Password, Salt 값 반환
        public Task<Tuple<string, string>> GetLoginDataAsync(string id, string pw);
    }
}