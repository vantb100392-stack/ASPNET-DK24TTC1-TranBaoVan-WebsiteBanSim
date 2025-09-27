namespace BBEcom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateImage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "Image", c => c.String());
            AlterColumn("dbo.Product", "Images", c => c.String());
            AlterColumn("dbo.News", "Image", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "Image", c => c.String(maxLength: 100));
            AlterColumn("dbo.Product", "Images", c => c.String(maxLength: 500));
            AlterColumn("dbo.Category", "Image", c => c.String(maxLength: 250));
        }
    }
}
