using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageUsers.Migrations
{
    /// <inheritdoc />
    public partial class AddSetadNameToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetadName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetadName",
                table: "Users");
        }
    }
}
