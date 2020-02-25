namespace KreemMachineLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class users : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        Email = c.String(nullable: false, unicode: false),
                        Hash = c.String(nullable: false, unicode: false),
                        role = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.role, cascadeDelete: true)
                .Index(t => t.Email, unique: true, name: "UQ_Email")
                .Index(t => t.role);
            
            CreateIndex("dbo.Roles", "Name", unique: true, name: "UQ_Name");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "role", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "role" });
            DropIndex("dbo.Users", "UQ_Email");
            DropIndex("dbo.Roles", "UQ_Name");
            DropTable("dbo.Users");
        }
    }
}
