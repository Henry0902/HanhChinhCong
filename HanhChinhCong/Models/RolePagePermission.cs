using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RolePagePermission")]
    public class RolePagePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string PageKey { get; set; }
    }


}