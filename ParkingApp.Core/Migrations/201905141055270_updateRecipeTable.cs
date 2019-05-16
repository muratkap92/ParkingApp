namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRecipeTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recipes", "Status1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "Status1", c => c.Boolean(nullable: false));
        }
    }
}
