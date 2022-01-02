using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableMail
    {
        public Int64 postID { get; set; }
        [StringLength(45)]
        public string ID { get; set; } = "";
        public Int32 StarCount { get; set; }
        public DateTime Date { get; set; }
    }
}