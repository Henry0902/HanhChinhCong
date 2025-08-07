using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("TrangThaiHoSo")]
    public class TrangThaiHoSo
    {
        public int Id { get; set; }
        public string TenTrangThai { get; set; }
    }

}