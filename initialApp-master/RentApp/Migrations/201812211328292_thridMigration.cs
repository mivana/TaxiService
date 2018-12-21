namespace RentApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class thridMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AppUsers", "JMBG", c => c.String());
            AlterColumn("dbo.AppUsers", "ContactNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AppUsers", "ContactNumber", c => c.Int());
            AlterColumn("dbo.AppUsers", "JMBG", c => c.Int());
        }
    }
}
