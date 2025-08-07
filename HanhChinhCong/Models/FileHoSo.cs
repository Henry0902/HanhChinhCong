using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("FileHoSo")]
    public class FileHoSo
    {
        public int Id { get; set; }
        public int IdHoSo { get; set; }
        public string TenFile { get; set; }
        public string DuongDan { get; set; }
    }


}