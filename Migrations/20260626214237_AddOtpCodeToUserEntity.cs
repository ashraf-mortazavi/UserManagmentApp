using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageUsers.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpCodeToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OTPCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendDateTimeOTPCode",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SendDateTimeOTPCode",
                table: "Users");
        }
    }
}
