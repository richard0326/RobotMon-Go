using System;
using System.Threading.Tasks;

namespace ApiServer
{
    public interface ICommonDb
    {
        // DB 열기.
        public void Open();
        
        // DB 닫기.
        public void Close();

        public Task<ServerCommon.ErrorCode> InsertCreateAccountDataAsync(string id, string pw, string salt);
        
        public Task<ServerCommon.ErrorCode> GetLoginDataAsync(string id, string pw);
    }
}