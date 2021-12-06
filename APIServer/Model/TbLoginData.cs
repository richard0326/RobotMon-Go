using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServer.Model
{
    public class TbLoginData
    {
        public string PW { get; set; }
        public string Salt { get; set; }
    }
}