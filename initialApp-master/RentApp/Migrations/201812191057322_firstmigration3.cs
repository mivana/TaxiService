namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstmigration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "Gender", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Gender");
        }
    }
}
