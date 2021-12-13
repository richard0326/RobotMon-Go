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
            if (String.CompareOrdinal(formString, "/Login") == 0 ||
                String.CompareOrdinal(formString, "/CreateAccount") == 0)
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
                return;
            }

            // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
            // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
            context.Request.EnableBuffering();
            string userAuthToken;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
            {
                var bodyStr = await reader.ReadToEndAsync();

                // body String에 어떤 문자열도 없다면...
                if (string.IsNullOrEmpty(bodyStr))
                {
                    return;
                }

                var document = JsonDocument.Parse(bodyStr);

                var userId = document.RootElement.GetProperty("ID").GetString();
                userAuthToken = document.RootElement.GetProperty("AuthToken").GetString();

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

                // Redis를 활용한 트랜잭션... 중복 처리 예방
                if (!await RedisDB.SetNxAsync(userAuthToken))
                {
                    return;
                }
            }

            context.Request.Body.Position = 0;
            
            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // 트랜잭션 해제(Redis 동기화 해제)
            await RedisDB.DelNxAsync(userAuthToken);
        }
    }
}