using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ClockInService
{
    public class ClockInEntity : DbContext
    {
        public ClockInEntity() : base("name=MyStrMssqlConn")
        {
        }
        public virtual DbSet<ClockModels> ClockModels { get; set; }

    }
}