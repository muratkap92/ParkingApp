namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubscriptionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "RecipeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Subscriptions", "RecipeId");
            AddForeignKey("dbo.Subscriptions", "RecipeId", "dbo.Recipes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "RecipeId", "dbo.Recipes");
            DropIndex("dbo.Subscriptions", new[] { "RecipeId" });
            DropColumn("dbo.Subscriptions", "RecipeId");
        }
    }
}
