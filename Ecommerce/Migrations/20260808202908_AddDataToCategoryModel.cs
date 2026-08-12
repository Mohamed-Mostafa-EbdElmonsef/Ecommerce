using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (Name, Description, Status) values ('Mobiles', 'Mauris lacinia sapien quis libero.', 0);insert into Categories (Name, Description, Status) values ('Laptops', 'Nulla ac enim.', 1);insert into Categories (Name, Description, Status) values ('Tablets', 'Aliquam erat volutpat.', 1);insert into Categories (Name, Description, Status) values ('Watches', 'Nullam sit amet turpis elementum ligula vehicula consequat.', 0);insert into Categories (Name, Description, Status) values ('Airbuds', 'In hac habitasse platea dictumst.', 1);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Categories");
        }
    }
}
