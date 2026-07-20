using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductsStockUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Products",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "Stock");
        }
    }
}
