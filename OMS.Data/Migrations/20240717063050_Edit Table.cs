using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Product",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderItem",
                newName: "OrderItemId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Order",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Invoice",
                newName: "InvoiceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customer",
                newName: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Product",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderItemId",
                table: "OrderItem",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Order",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Invoice",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customer",
                newName: "Id");
        }
    }
}
