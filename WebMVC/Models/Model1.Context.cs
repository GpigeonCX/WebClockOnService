﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebMVC.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Auth2015Entities : DbContext
    {
        public Auth2015Entities()
            : base("name=Auth2015Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BaseException> BaseException { get; set; }
        public virtual DbSet<BaseItemCategory> BaseItemCategory { get; set; }
        public virtual DbSet<BaseItems> BaseItems { get; set; }
        public virtual DbSet<BaseLog> BaseLog { get; set; }
        public virtual DbSet<BaseModule> BaseModule { get; set; }
        public virtual DbSet<BaseOrganize> BaseOrganize { get; set; }
        public virtual DbSet<BaseRelation> BaseRelation { get; set; }
        public virtual DbSet<BaseRole> BaseRole { get; set; }
        public virtual DbSet<BaseStaff> BaseStaff { get; set; }
        public virtual DbSet<BaseUser> BaseUser { get; set; }
        public virtual DbSet<BaseWorkGroup> BaseWorkGroup { get; set; }
    }
}
