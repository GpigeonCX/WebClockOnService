using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebMVC.Models.BaseTB;

namespace WebMVC.Models
{
    public class ClockInEntity : DbContext
    {
        public ClockInEntity() : base("name=MyStrMssqlConn")
        {
        }
        public virtual DbSet<ClockModels> ClockModels { get; set; }
        public virtual DbSet<UserModels> UserModels { get; set; }

    }
}