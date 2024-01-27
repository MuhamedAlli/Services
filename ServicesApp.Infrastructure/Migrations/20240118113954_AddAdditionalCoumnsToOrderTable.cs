using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalCoumnsToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_AcceptedById",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AcceptedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AcceptedById",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "Orders",
                newName: "OrderTitle");

            migrationBuilder.AlterColumn<string>(
                name: "OrderTime",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "MaterialsPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VisitPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProviderId",
                table: "Orders",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ProviderId",
                table: "Orders",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ProviderId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProviderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MaterialsPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VisitPrice",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderTitle",
                table: "Orders",
                newName: "JobTitle");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderTime",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AcceptedById",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AcceptedById",
                table: "Orders",
                column: "AcceptedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_AcceptedById",
                table: "Orders",
                column: "AcceptedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
