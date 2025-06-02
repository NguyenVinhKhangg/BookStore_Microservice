using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class deactivateStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "DeactivatedStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeactivatedStatus",
                table: "Users",
                newName: "IsActive");
        }
    }
}
