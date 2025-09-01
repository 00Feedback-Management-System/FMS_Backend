using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class RemainingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseGroups",
                columns: table => new
                {
                    course_group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    group_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseGroups", x => x.course_group_id);
                    table.ForeignKey(
                        name: "FK_CourseGroups_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseGroups_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseGroups_Modules_module_id",
                        column: x => x.module_id,
                        principalTable: "Modules",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudents",
                columns: table => new
                {
                    course_student_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    student_rollno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudents", x => x.course_student_id);
                    table.ForeignKey(
                        name: "FK_CourseStudents_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudents_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    role_access_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffrole_id = table.Column<int>(type: "int", nullable: false),
                    student_rollno = table.Column<int>(type: "int", nullable: false),
                    route = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    component = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.role_access_id);
                    table.ForeignKey(
                        name: "FK_RoleAccesses_Staffroles_staffrole_id",
                        column: x => x.staffrole_id,
                        principalTable: "Staffroles",
                        principalColumn: "staffrole_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAccesses_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_course_id",
                table: "CourseGroups",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_group_id",
                table: "CourseGroups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_module_id",
                table: "CourseGroups",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_course_id",
                table: "CourseStudents",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_student_rollno",
                table: "CourseStudents",
                column: "student_rollno");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_staffrole_id",
                table: "RoleAccesses",
                column: "staffrole_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_student_rollno",
                table: "RoleAccesses",
                column: "student_rollno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseGroups");

            migrationBuilder.DropTable(
                name: "CourseStudents");

            migrationBuilder.DropTable(
                name: "RoleAccesses");
        }
    }
}
