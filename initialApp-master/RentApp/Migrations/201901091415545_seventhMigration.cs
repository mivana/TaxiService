namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seventhMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Username");
        }
    }
}
