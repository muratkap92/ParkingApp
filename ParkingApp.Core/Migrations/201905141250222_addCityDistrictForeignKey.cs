namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCityDistrictForeignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "City_Id", c => c.Int());
            CreateIndex("dbo.Districts", "City_Id");
            AddForeignKey("dbo.Districts", "City_Id", "dbo.Cities", "Id");
            DropColumn("dbo.Districts", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Districts", "CityId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Districts", "City_Id", "dbo.Cities");
            DropIndex("dbo.Districts", new[] { "City_Id" });
            DropColumn("dbo.Districts", "City_Id");
        }
    }
}
