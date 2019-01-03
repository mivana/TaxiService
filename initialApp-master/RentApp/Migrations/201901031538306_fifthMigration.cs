namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fifthMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rides", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rides", "Status", c => c.String());
        }
    }
}
