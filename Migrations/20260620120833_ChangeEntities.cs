using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageUsers.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NextLowerRoleId",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NextLowerRoleId",
                table: "Roles",
                column: "NextLowerRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Roles_NextLowerRoleId",
                table: "Roles",
                column: "NextLowerRoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Roles_NextLowerRoleId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_NextLowerRoleId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "NextLowerRoleId",
                table: "Roles");
        }
    }
}
