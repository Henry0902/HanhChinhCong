using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public int VaiTro { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}