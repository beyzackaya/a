using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdminToGroupUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "GroupUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "GroupUsers");
        }
    }
}
