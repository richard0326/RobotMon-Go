using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiServer.Services
{
    public class AuthTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuthTokenCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var formString = context.Request.Path.Value;
            if (String.CompareOrdinal(formString, "/Login") == 0 || String.CompareOrdinal(formString, "/CreateAccount") == 0)
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
                return;
            }
            
            // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
            // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                var bodyStr = await reader.ReadToEndAsync();
                
                // body String에 어떤 문자열도 없다면...
                if (string.IsNullOrEmpty(bodyStr))      
                {
                    return;
                }

                var startId = bodyStr.IndexOf($"ID");
                // ": "  ":"  " :" 의 상황을 피하기 위해서 2번 진행
                startId = bodyStr.IndexOf('"', startId) + 1; 
                startId = bodyStr.IndexOf('"', startId) + 1;
                var endId = bodyStr.IndexOf('"', startId);
                var userId = bodyStr.Substring(startId, endId - startId);

                var startAuthToken = bodyStr.IndexOf("AuthToken");
                // ": "  ":"  " :" 의 상황을 피하기 위해서 2번 진행
                startAuthToken = bodyStr.IndexOf('"', startAuthToken) + 1;
                startAuthToken = bodyStr.IndexOf('"', startAuthToken) + 1;
                var endAuthToken = bodyStr.IndexOf('"', startAuthToken);

                var userAuthToken = bodyStr.Substring(startAuthToken, endAuthToken - startAuthToken);

                // redis에서 로그인 유저 정보 받아오기... 없으면 로그인 성공한 유저가 아님.
                var userInfo = await RedisDB.GetUserInfo(userId);
                if (userInfo == null)
                {
                    return;
                }
            
                // id, AuthToken 일치 여부 확인...
                if (String.CompareOrdinal(userInfo.AuthToken, userAuthToken) != 0)
                {
                    return;
                }
            }

            context.Request.Body.Position = 0;
            // Call the next delegate/middleware in the pipeline
             await _next(context);
        }
    }
}