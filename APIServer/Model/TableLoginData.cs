using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServer.Model
{
    public class TableLoginData
    {
        public Int64 UID { get; set; }
        public string? PW { get; set; }
        public string? Salt { get; set; }
    }
}