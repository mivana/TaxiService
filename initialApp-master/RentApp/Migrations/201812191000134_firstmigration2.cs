namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstmigration2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AppUsers", "DriverLocationId", "dbo.Locations");
            DropIndex("dbo.AppUsers", new[] { "DriverLocationId" });
            DropIndex("dbo.Locations", new[] { "AddressId" });
            AddColumn("dbo.Addresses", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUsers", "DriverFree", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUsers", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AppUsers", "Blocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Rides", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Locations", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cars", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cars", "AppUserID", c => c.Int(nullable: false));
            AlterColumn("dbo.AppUsers", "JMBG", c => c.Int());
            AlterColumn("dbo.AppUsers", "ContactNumber", c => c.Int());
            AlterColumn("dbo.AppUsers", "Email", c => c.String(maxLength: 100, unicode: false));
            AlterColumn("dbo.AppUsers", "DriverLocationId", c => c.Int(nullable: true));
            AlterColumn("dbo.Rides", "Status", c => c.String());
            CreateIndex("dbo.AppUsers", "DriverLocationId");
            CreateIndex("dbo.Locations", "AddressID");
            CreateIndex("dbo.Cars", "AppUserID");
            AddForeignKey("dbo.Cars", "AppUserID", "dbo.AppUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AppUsers", "DriverLocationId", "dbo.Locations", "Id");
            DropColumn("dbo.AppUsers", "UserName");
            DropColumn("dbo.AppUsers", "Gender");
            DropColumn("dbo.Cars", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cars", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.AppUsers", "Gender", c => c.Int(nullable: false));
            AddColumn("dbo.AppUsers", "UserName", c => c.String());
            DropForeignKey("dbo.AppUsers", "DriverLocationId", "dbo.Locations");
            DropForeignKey("dbo.Cars", "AppUserID", "dbo.AppUsers");
            DropIndex("dbo.Cars", new[] { "AppUserID" });
            DropIndex("dbo.Locations", new[] { "AddressID" });
            DropIndex("dbo.AppUsers", new[] { "DriverLocationId" });
            AlterColumn("dbo.Rides", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.AppUsers", "DriverLocationId", c => c.Int(nullable: false));
            AlterColumn("dbo.AppUsers", "Email", c => c.String());
            AlterColumn("dbo.AppUsers", "ContactNumber", c => c.String());
            AlterColumn("dbo.AppUsers", "JMBG", c => c.String());
            DropColumn("dbo.Cars", "AppUserID");
            DropColumn("dbo.Cars", "Deleted");
            DropColumn("dbo.Comments", "Deleted");
            DropColumn("dbo.Locations", "Deleted");
            DropColumn("dbo.Rides", "Deleted");
            DropColumn("dbo.AppUsers", "Blocked");
            DropColumn("dbo.AppUsers", "Deleted");
            DropColumn("dbo.AppUsers", "DriverFree");
            DropColumn("dbo.Addresses", "Deleted");
            CreateIndex("dbo.Locations", "AddressId");
            CreateIndex("dbo.AppUsers", "DriverLocationId");
            AddForeignKey("dbo.AppUsers", "DriverLocationId", "dbo.Locations", "Id", cascadeDelete: true);
        }
    }
}
