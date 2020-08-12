namespace Rental.BLL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                {
                    CompanyId = c.Guid(nullable: false),
                    CompanyName = c.String(nullable: false, maxLength: 50),
                    PostCode = c.String(),
                    City = c.String(),
                    Address = c.String(),
                    Email = c.String(),
                    Phone = c.String(),
                    CompanyType = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.CompanyId);

            CreateTable(
                "dbo.Tools",
                c => new
                {
                    ToolId = c.Guid(nullable: false),
                    Name = c.String(maxLength: 50),
                    Sn = c.String(),
                    PurchaseDate = c.DateTime(nullable: false),
                    PurchasesValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DocumentNumber = c.String(),
                    Warranty = c.Int(nullable: false),
                    RentalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Destroyed = c.Boolean(nullable: false),
                    DestroyedDate = c.DateTime(),
                    Lost = c.Boolean(nullable: false),
                    LostDate = c.DateTime(),
                    Description = c.String(),
                    DestroyedCustomer_UserId = c.Guid(),
                    LostCustomer_UserId = c.Guid(),
                    Producer_CompanyId = c.Guid(),
                })
                .PrimaryKey(t => t.ToolId)
                .ForeignKey("dbo.Users", t => t.DestroyedCustomer_UserId)
                .ForeignKey("dbo.Users", t => t.LostCustomer_UserId)
                .ForeignKey("dbo.Companies", t => t.Producer_CompanyId)
                .Index(t => t.DestroyedCustomer_UserId)
                .Index(t => t.LostCustomer_UserId)
                .Index(t => t.Producer_CompanyId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    UserId = c.Guid(nullable: false),
                    FirstName = c.String(nullable: false, maxLength: 50),
                    LastName = c.String(maxLength: 50),
                    Phone = c.String(),
                    Email = c.String(),
                    LoginName = c.String(maxLength: 50),
                    Password = c.String(maxLength: 50),
                    UserType = c.Int(nullable: false),
                    Company_CompanyId = c.Guid(),
                })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Companies", t => t.Company_CompanyId)
                .Index(t => t.Company_CompanyId);

            CreateTable(
                "dbo.Transactions",
                c => new
                {
                    TransactionId = c.Guid(nullable: false),
                    TransactionType = c.Int(nullable: false),
                    TransactionDate = c.DateTime(nullable: false),
                    TransactionNumber = c.Int(nullable: false),
                    PriceForRent = c.Decimal(nullable: false, precision: 18, scale: 2),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    AppUser_UserId = c.Guid(nullable: false),
                    Customer_UserId = c.Guid(nullable: false),
                    Tool_ToolId = c.Guid(nullable: false),
                    User_UserId = c.Guid(),
                })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Users", t => t.AppUser_UserId)
                .ForeignKey("dbo.Users", t => t.Customer_UserId)
                .ForeignKey("dbo.Tools", t => t.Tool_ToolId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.AppUser_UserId)
                .Index(t => t.Customer_UserId)
                .Index(t => t.Tool_ToolId)
                .Index(t => t.User_UserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Tools", "Producer_CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Tools", "LostCustomer_UserId", "dbo.Users");
            DropForeignKey("dbo.Tools", "DestroyedCustomer_UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "Tool_ToolId", "dbo.Tools");
            DropForeignKey("dbo.Transactions", "Customer_UserId", "dbo.Users");
            DropForeignKey("dbo.Transactions", "AppUser_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "Company_CompanyId", "dbo.Companies");
            DropIndex("dbo.Transactions", new[] { "User_UserId" });
            DropIndex("dbo.Transactions", new[] { "Tool_ToolId" });
            DropIndex("dbo.Transactions", new[] { "Customer_UserId" });
            DropIndex("dbo.Transactions", new[] { "AppUser_UserId" });
            DropIndex("dbo.Users", new[] { "Company_CompanyId" });
            DropIndex("dbo.Tools", new[] { "Producer_CompanyId" });
            DropIndex("dbo.Tools", new[] { "LostCustomer_UserId" });
            DropIndex("dbo.Tools", new[] { "DestroyedCustomer_UserId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.Users");
            DropTable("dbo.Tools");
            DropTable("dbo.Companies");
        }
    }
}
