namespace ParkingApp.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRecipeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Controls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        License = c.String(),
                        EnterDate = c.DateTime(nullable: false),
                        ExitDate = c.DateTime(nullable: false),
                        Time = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        MinimumValue = c.Int(nullable: false),
                        MaximumValue = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        Status1 = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subscribers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        License = c.String(),
                        City = c.String(),
                        District = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubscriberId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Cost = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subscribers", t => t.SubscriberId, cascadeDelete: true)
                .Index(t => t.SubscriberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "SubscriberId", "dbo.Subscribers");
            DropIndex("dbo.Subscriptions", new[] { "SubscriberId" });
            DropTable("dbo.Subscriptions");
            DropTable("dbo.Subscribers");
            DropTable("dbo.Recipes");
            DropTable("dbo.Districts");
            DropTable("dbo.Controls");
            DropTable("dbo.Cities");
        }
    }
}
