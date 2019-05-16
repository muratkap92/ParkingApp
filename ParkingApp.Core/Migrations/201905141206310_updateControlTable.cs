namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateControlTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Controls", "ExitDate", c => c.DateTime());
            AlterColumn("dbo.Controls", "Time", c => c.Int());
            AlterColumn("dbo.Controls", "Cost", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Controls", "Cost", c => c.Int(nullable: false));
            AlterColumn("dbo.Controls", "Time", c => c.Int(nullable: false));
            AlterColumn("dbo.Controls", "ExitDate", c => c.DateTime(nullable: false));
        }
    }
}
