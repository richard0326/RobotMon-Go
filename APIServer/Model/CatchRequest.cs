namespace ApiServer.Model
{
    public class CatchRequest
    {
        public string ID { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public Int64 MonsterID { get; set; }
    }
}