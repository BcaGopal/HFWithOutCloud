namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentTypeProcess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Web.DocumentTypeProcesses",
                c => new
                    {
                        DocumentTypeProcessId = c.Int(nullable: false, identity: true),
                        DocumentTypeId = c.Int(nullable: false),
                        ProcessId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        OMSId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.DocumentTypeProcessId)
                .ForeignKey("Web.DocumentTypes", t => t.DocumentTypeId)
                .ForeignKey("Web.Processes", t => t.ProcessId)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.ProcessId);
            
            AddColumn("Web.JobOrderSettings", "IsMandatoryStockIn", c => c.Boolean());
            AddColumn("Web.StockHeaderSettings", "IsMandatoryStockIn", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("Web.DocumentTypeProcesses", "ProcessId", "Web.Processes");
            DropForeignKey("Web.DocumentTypeProcesses", "DocumentTypeId", "Web.DocumentTypes");
            DropIndex("Web.DocumentTypeProcesses", new[] { "ProcessId" });
            DropIndex("Web.DocumentTypeProcesses", new[] { "DocumentTypeId" });
            DropColumn("Web.StockHeaderSettings", "IsMandatoryStockIn");
            DropColumn("Web.JobOrderSettings", "IsMandatoryStockIn");
            DropTable("Web.DocumentTypeProcesses");
        }
    }
}
