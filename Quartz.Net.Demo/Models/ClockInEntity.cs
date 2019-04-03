using Quartz.Net.Demo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Quartz.Net.Demo
{
    public class ClockInEntity : DbContext
    {
        public ClockInEntity() : base("name=MyStrMssqlConn")
        {
        }
        public virtual DbSet<ClockModels> ClockModels { get; set; }
        public virtual DbSet<ClockBatch> ClockBatch { get; set; }
    }
}