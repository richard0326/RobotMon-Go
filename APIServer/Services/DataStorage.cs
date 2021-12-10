using System.Collections.Concurrent;
using ApiServer.Model;
using Dapper;
using MySqlConnector;

namespace ApiServer.Services
{

    public class DataStorage
    {
        static ConcurrentDictionary<Int64, Monster> MonsterDic = new();

        public static void Load(string dbConnString)
        {
            using var dBConn = new MySqlConnection(dbConnString);
            dBConn.Open();

            var valueList = dBConn.Query<TableMonsterInfo>("select * from monsterinfo");
            foreach (var value in valueList)
            {
                MonsterDic.TryAdd(value.MID, new Monster()
                {
                    Att = value.Att,
                    Def = value.Def,
                    HP = value.HP,
                    Level = value.Level,
                    MonsterName = value.MonsterName,
                    StarCount = value.StarCount,
                    Type = value.Type
                });
            }
        }
        

        public static Monster GetMonsterInfo(Int64 monsterID)
        {
            if(MonsterDic.TryGetValue(monsterID, out var value))
            {
                return value;
            }

            return null;
        }       
    }
}