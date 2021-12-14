using System;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableUserGameInfo
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        public Int64 UserLevel { get; set; }
        public Int64 UserExp { get; set; }
        public Int64 StarPoint { get; set; } // 별의 모래
        public Int64 RankPoint { get; set; } // 랭킹을 위한 점수
    }
}