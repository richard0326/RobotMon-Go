using ServerCommon;

namespace ApiServer.Model
{
    public class LoginResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public string? Authtoken { get; set; }
    }
}