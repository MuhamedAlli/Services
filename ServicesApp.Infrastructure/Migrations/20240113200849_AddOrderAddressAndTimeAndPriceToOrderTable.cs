using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAddressAndTimeAndPriceToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobDesc",
                table: "Orders",
                newName: "OrderDescription");

            migrationBuilder.AddColumn<string>(
                name: "OrderAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderTime",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderTime",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderDescription",
                table: "Orders",
                newName: "JobDesc");
        }
    }
}
