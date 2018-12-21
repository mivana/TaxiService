namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secondMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Username");
        }
    }
}
