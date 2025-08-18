using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("LinhVuc")]
    public class LinhVuc
    {
        public int Id { get; set; }
        public string TenLinhVuc { get; set; }
        public int IdPhongBan { get; set; }
        public bool Active { get; set; }

    }

    public class LinhVucViewModel
    {
        public int Id { get; set; }
        public string TenLinhVuc { get; set; }
        public int IdPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public bool Active { get; set; }
        // Thêm các trường khác nếu cần
    }


}