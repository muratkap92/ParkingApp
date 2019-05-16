namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubscription1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "IsPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscriptions", "IsPaid");
        }
    }
}
