using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HanhChinhCong.Models
{
    [Table("PhongBan")]
    public class PhongBan
    {
        public int Id { get; set; }
        public string TenPhongBan { get; set; }
        public bool Active { get; set; }
    }
}