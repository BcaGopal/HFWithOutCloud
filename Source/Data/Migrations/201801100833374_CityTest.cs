namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CityTest : DbMigration
    {
        public override void Up()
        {
            DropIndex("Web.RolesDocTypes", new[] { "DocTypeId" });
            AddColumn("Web.Menus", "DocumentCategoryId", c => c.Int());
            AddColumn("Web.Cities", "Test", c => c.String(maxLength: 50));
            AddColumn("Web.RolesDocTypes", "MenuId", c => c.Int());
            AlterColumn("Web.RolesDocTypes", "DocTypeId", c => c.Int());
            CreateIndex("Web.Menus", "DocumentCategoryId");
            CreateIndex("Web.RolesDocTypes", "DocTypeId");
            CreateIndex("Web.RolesDocTypes", "MenuId");
            AddForeignKey("Web.Menus", "DocumentCategoryId", "Web.DocumentCategories", "DocumentCategoryId");
            AddForeignKey("Web.RolesDocTypes", "MenuId", "Web.Menus", "MenuId");
            DropTable("Web.SampleTables");
        }
        
        public override void Down()
        {
            CreateTable(
                "Web.SampleTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("Web.RolesDocTypes", "MenuId", "Web.Menus");
            DropForeignKey("Web.Menus", "DocumentCategoryId", "Web.DocumentCategories");
            DropIndex("Web.RolesDocTypes", new[] { "MenuId" });
            DropIndex("Web.RolesDocTypes", new[] { "DocTypeId" });
            DropIndex("Web.Menus", new[] { "DocumentCategoryId" });
            AlterColumn("Web.RolesDocTypes", "DocTypeId", c => c.Int(nullable: false));
            DropColumn("Web.RolesDocTypes", "MenuId");
            DropColumn("Web.Cities", "Test");
            DropColumn("Web.Menus", "DocumentCategoryId");
            CreateIndex("Web.RolesDocTypes", "DocTypeId");
        }
    }
}
