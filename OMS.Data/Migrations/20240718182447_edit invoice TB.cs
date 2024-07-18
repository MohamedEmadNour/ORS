using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class editinvoiceTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invoice");
        }
    }
}
