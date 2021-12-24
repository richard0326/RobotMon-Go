using System.Collections.Concurrent;
using ApiServer.Model;
using Dapper;
using MySqlConnector;

namespace ApiServer.Services
{

    public class DataStorage
    {
        private static ConcurrentDictionary<Int64, Monster> s_monsterDic = new();
        private static ConcurrentDictionary<Int64, DailyInfo> s_dailyCheckDic = new();

        public static void Load(string dbConnString)
        {
            using var dBConn = new MySqlConnection(dbConnString);
            {
                dBConn.Open();

                var monsterList = dBConn.Query<TableMonsterInfo>("select * from monsterinfo");
                foreach (var value in monsterList)
                {
                    s_monsterDic.TryAdd(value.MID, new Monster()
                    {
                        Att = value.Att,
                        Def = value.Def,
                        HP = value.HP,
                        Level = value.Level,
                        MonsterName = value.MonsterName,
                        StarCount = value.StarCount,
                        UpgradeCount = value.UpgradeCount,
                        Type = value.Type
                    });
                }

                var dailyCheckList = dBConn.Query<TableDailyInfo>("select * from DailyInfo");
                foreach (var value in dailyCheckList)
                {
                    s_dailyCheckDic.TryAdd(value.DayCount, new DailyInfo()
                    {
                        StarCount = value.StarCount
                    });
                }
            }
        }
        
        public static Monster GetMonsterInfo(Int64 monsterID)
        {
            if(s_monsterDic.TryGetValue(monsterID, out var value))
            {
                return value;
            }

            return null;
        }
        
        public static DailyInfo GetDailyInfo(Int32 dailyIdx)
        {
            if(s_dailyCheckDic.TryGetValue(dailyIdx, out var value))
            {
                return value;
            }

            return null;
        }    
    }
}