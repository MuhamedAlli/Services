using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSomeColsInUsersAndOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerRate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "WorkerComment",
                table: "Orders",
                newName: "ProviderComment");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClientRate",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 3.5m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProviderRate",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 3.5m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderRate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ProviderComment",
                table: "Orders",
                newName: "WorkerComment");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClientRate",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 3.5m);

            migrationBuilder.AddColumn<decimal>(
                name: "WorkerRate",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
