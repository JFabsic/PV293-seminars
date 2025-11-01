using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yestino.ProductCatalog.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "product_catalog",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "product_catalog",
                table: "Products");
        }
    }
}
