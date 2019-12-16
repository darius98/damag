namespace Damag.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Something : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rating = c.Int(nullable: false),
                        Text = c.String(maxLength: 1024),
                        UserId = c.String(maxLength: 128),
                        ItemId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "ItemId", "dbo.Items");
            DropIndex("dbo.Reviews", new[] { "ItemId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropTable("dbo.Reviews");
        }
    }
}
