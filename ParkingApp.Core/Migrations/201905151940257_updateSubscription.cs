namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubscription : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Subscriptions", "Cost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "Cost", c => c.Int(nullable: false));
        }
    }
}
