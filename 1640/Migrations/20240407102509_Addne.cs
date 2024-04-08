using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640.Migrations
{
    /// <inheritdoc />
    public partial class Addne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Semesters_Faculties_FacultyId",
                table: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Semesters_FacultyId",
                table: "Semesters");

            migrationBuilder.AddColumn<string>(
                name: "FacultyName",
                table: "Semesters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacultyName",
                table: "Semesters");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_FacultyId",
                table: "Semesters",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Semesters_Faculties_FacultyId",
                table: "Semesters",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
