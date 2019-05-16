namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Districts", "City_Id", "dbo.Cities");
            DropIndex("dbo.Districts", new[] { "City_Id" });
            AlterColumn("dbo.Districts", "City_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Districts", "City_Id", c => c.Int());
            CreateIndex("dbo.Districts", "City_Id");
            AddForeignKey("dbo.Districts", "City_Id", "dbo.Cities", "Id");
        }
    }
}
