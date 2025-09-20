using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class RoleAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccesses_Students_student_rollno",
                table: "RoleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_RoleAccesses_student_rollno",
                table: "RoleAccesses");

            migrationBuilder.DropColumn(
                name: "student_rollno",
                table: "RoleAccesses");

            migrationBuilder.AddColumn<int>(
                name: "Studentsstudent_rollno",
                table: "RoleAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_Studentsstudent_rollno",
                table: "RoleAccesses",
                column: "Studentsstudent_rollno");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_Students_Studentsstudent_rollno",
                table: "RoleAccesses",
                column: "Studentsstudent_rollno",
                principalTable: "Students",
                principalColumn: "student_rollno",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccesses_Students_Studentsstudent_rollno",
                table: "RoleAccesses");

            migrationBuilder.DropIndex(
                name: "IX_RoleAccesses_Studentsstudent_rollno",
                table: "RoleAccesses");

            migrationBuilder.DropColumn(
                name: "Studentsstudent_rollno",
                table: "RoleAccesses");

            migrationBuilder.AddColumn<int>(
                name: "student_rollno",
                table: "RoleAccesses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_student_rollno",
                table: "RoleAccesses",
                column: "student_rollno");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccesses_Students_student_rollno",
                table: "RoleAccesses",
                column: "student_rollno",
                principalTable: "Students",
                principalColumn: "student_rollno");
        }
    }
}
