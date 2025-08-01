using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrivateChatToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivateChat",
                table: "Groups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivateChat",
                table: "Groups");
        }
    }
}
