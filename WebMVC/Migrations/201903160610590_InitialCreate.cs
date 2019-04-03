namespace WebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClockModels",
                c => new
                    {
                        CardId = c.String(nullable: false, maxLength: 128),
                        EmployeeName = c.String(),
                        LastClockTime = c.DateTime(),
                        ClockStateAM = c.Boolean(nullable: false),
                        ClockStatePM = c.Boolean(nullable: false),
                        TotalDays = c.Int(nullable: false),
                        CreatTime = c.DateTime(nullable: false),
                        OnTimeStart = c.DateTime(nullable: false),
                        OnTimeEnd = c.DateTime(nullable: false),
                        OffTimeStart = c.DateTime(nullable: false),
                        OffTimeEnd = c.DateTime(nullable: false),
                        NeedSta = c.Boolean(nullable: false),
                        NeedSun = c.Boolean(nullable: false),
                        flag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CardId);
            
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        UserPwd = c.String(),
                    })
                .PrimaryKey(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserModels");
            DropTable("dbo.ClockModels");
        }
    }
}
