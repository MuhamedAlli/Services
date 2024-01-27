using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditStateColumnNameInAppUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Satet",
                table: "AspNetUsers",
                newName: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "Satet");
        }
    }
}
