using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("LoaiHoSo")]
    public class LoaiHoSo
    {
        public int Id { get; set; }
        public string TenLoaiHoSo { get; set; }
        public int IdLinhVuc { get; set; }
        public string MaLoaiHoSo { get; set; }
    }

    public class LoaiHoSoViewModel
    {
        public int Id { get; set; }
        public string TenLoaiHoSo { get; set; }
        public int IdLinhVuc { get; set; }
        public string TenLinhVuc { get; set; }
        public string MaLoaiHoSo { get; set; }
    }



}