using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOtpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentDate",
                table: "Otps",
                newName: "ExpireDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                table: "Otps",
                newName: "SentDate");
        }
    }
}
