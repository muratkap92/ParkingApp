namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForeignKeys1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Districts", "City_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Districts", "City_Id", c => c.Int(nullable: false));
        }
    }
}
