using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class course_group : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Modules_module_id",
                table: "CourseGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Courses_course_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_FeedbackType_feedback_type_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Modules_module_id",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_CourseGroups_module_id",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "module_id",
                table: "CourseGroups");

            migrationBuilder.AddColumn<int>(
                name: "course_id",
                table: "Modules",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "module_id",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "feedback_type_id",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "course_id",
                table: "Feedback",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_course_id",
                table: "Modules",
                column: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Courses_course_id",
                table: "Feedback",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "course_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_FeedbackType_feedback_type_id",
                table: "Feedback",
                column: "feedback_type_id",
                principalTable: "FeedbackType",
                principalColumn: "feedback_type_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Modules_module_id",
                table: "Feedback",
                column: "module_id",
                principalTable: "Modules",
                principalColumn: "module_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Courses_course_id",
                table: "Modules",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "course_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Courses_course_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_FeedbackType_feedback_type_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Modules_module_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Courses_course_id",
                table: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Modules_course_id",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "Modules");

            migrationBuilder.AlterColumn<int>(
                name: "module_id",
                table: "Feedback",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "feedback_type_id",
                table: "Feedback",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "course_id",
                table: "Feedback",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "module_id",
                table: "CourseGroups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_module_id",
                table: "CourseGroups",
                column: "module_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Modules_module_id",
                table: "CourseGroups",
                column: "module_id",
                principalTable: "Modules",
                principalColumn: "module_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Courses_course_id",
                table: "Feedback",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_FeedbackType_feedback_type_id",
                table: "Feedback",
                column: "feedback_type_id",
                principalTable: "FeedbackType",
                principalColumn: "feedback_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Modules_module_id",
                table: "Feedback",
                column: "module_id",
                principalTable: "Modules",
                principalColumn: "module_id");
        }
    }
}
