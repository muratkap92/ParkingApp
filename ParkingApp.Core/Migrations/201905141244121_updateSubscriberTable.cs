namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubscriberTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscribers", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.Subscribers", "DistrictId", c => c.Int(nullable: false));
            CreateIndex("dbo.Subscribers", "CityId");
            CreateIndex("dbo.Subscribers", "DistrictId");
            AddForeignKey("dbo.Subscribers", "CityId", "dbo.Cities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Subscribers", "DistrictId", "dbo.Districts", "Id", cascadeDelete: true);
            DropColumn("dbo.Subscribers", "City");
            DropColumn("dbo.Subscribers", "District");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscribers", "District", c => c.String());
            AddColumn("dbo.Subscribers", "City", c => c.String());
            DropForeignKey("dbo.Subscribers", "DistrictId", "dbo.Districts");
            DropForeignKey("dbo.Subscribers", "CityId", "dbo.Cities");
            DropIndex("dbo.Subscribers", new[] { "DistrictId" });
            DropIndex("dbo.Subscribers", new[] { "CityId" });
            DropColumn("dbo.Subscribers", "DistrictId");
            DropColumn("dbo.Subscribers", "CityId");
        }
    }
}
