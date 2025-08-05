using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HanhChinhCong.Models
{
    public class DbConnectContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }
    }
}