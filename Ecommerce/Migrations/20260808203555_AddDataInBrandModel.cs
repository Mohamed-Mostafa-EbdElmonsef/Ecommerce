using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (Name, Description, Status) values ('Apple', 'Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci.', 1);insert into Brands (Name, Description, Status) values ('Oppo', 'Suspendisse accumsan tortor quis turpis.', 1);insert into Brands (Name, Description, Status) values ('Realme', 'Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec pharetra, magna vestibulum aliquet ultrices, erat tortor sollicitudin mi, sit amet lobortis sapien sapien non mi.', 1);insert into Brands (Name, Description, Status) values ('Dell', 'Nulla facilisi.', 0);insert into Brands (Name, Description, Status) values ('HP', 'Aenean sit amet justo.', 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Brands");
        }
    }
}
