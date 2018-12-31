namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourthMigration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cars", "TaxiNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cars", "TaxiNumber", c => c.Int(nullable: false));
        }
    }
}
