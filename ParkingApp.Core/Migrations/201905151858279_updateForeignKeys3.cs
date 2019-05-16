namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForeignKeys3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Districts", "City_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Districts", "City_Id");
        }
    }
}
