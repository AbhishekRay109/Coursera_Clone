using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kitana.Api.Migrations
{
    /// <inheritdoc />
    public partial class SkillForge3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTill",
                table: "UserCourse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ActiveTime",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "CertificateFile",
                table: "Certificates",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTill",
                table: "UserCourse");

            migrationBuilder.DropColumn(
                name: "ActiveTime",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CertificateFile",
                table: "Certificates");
        }
    }
}
