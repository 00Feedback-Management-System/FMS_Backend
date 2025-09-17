using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    course_type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.course_id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackType",
                columns: table => new
                {
                    feedback_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    feedback_type_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_type_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_module = table.Column<bool>(type: "bit", nullable: false),
                    group = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_staff = table.Column<bool>(type: "bit", nullable: false),
                    is_session = table.Column<bool>(type: "bit", nullable: false),
                    behaviour = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackType", x => x.feedback_type_id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    group_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "Staffroles",
                columns: table => new
                {
                    staffrole_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffrole_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffroles", x => x.staffrole_id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    module_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.module_id);
                    table.ForeignKey(
                        name: "FK_Modules_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackQuestions",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    question_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_type_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackQuestions", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_FeedbackQuestions_FeedbackType_feedback_type_id",
                        column: x => x.feedback_type_id,
                        principalTable: "FeedbackType",
                        principalColumn: "feedback_type_id");
                });

            migrationBuilder.CreateTable(
                name: "CourseGroups",
                columns: table => new
                {
                    course_group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    group_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseGroups", x => x.course_group_id);
                    table.ForeignKey(
                        name: "FK_CourseGroups_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK_CourseGroups_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    student_rollno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    group_id = table.Column<int>(type: "int", nullable: true),
                    profile_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    login_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.student_rollno);
                    table.ForeignKey(
                        name: "FK_Students_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id");
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    staff_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffrole_id = table.Column<int>(type: "int", nullable: true),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profile_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    login_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_Staff_Staffroles_staffrole_id",
                        column: x => x.staffrole_id,
                        principalTable: "Staffroles",
                        principalColumn: "staffrole_id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    feedback_type_id = table.Column<int>(type: "int", nullable: false),
                    session = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK_Feedback_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_FeedbackType_feedback_type_id",
                        column: x => x.feedback_type_id,
                        principalTable: "FeedbackType",
                        principalColumn: "feedback_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedback_Modules_module_id",
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
                    course_id = table.Column<int>(type: "int", nullable: true),
                    student_rollno = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudents", x => x.course_student_id);
                    table.ForeignKey(
                        name: "FK_CourseStudents_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK_CourseStudents_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno");
                });

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                columns: table => new
                {
                    role_access_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffrole_id = table.Column<int>(type: "int", nullable: true),
                    student_rollno = table.Column<int>(type: "int", nullable: true),
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
                        principalColumn: "staffrole_id");
                    table.ForeignKey(
                        name: "FK_RoleAccesses_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackGroup",
                columns: table => new
                {
                    FeedbackGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackGroup", x => x.FeedbackGroupId);
                    table.ForeignKey(
                        name: "FK_FeedbackGroup_Feedback_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedback",
                        principalColumn: "feedback_id");
                    table.ForeignKey(
                        name: "FK_FeedbackGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK_FeedbackGroup_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "staff_id");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackSubmits",
                columns: table => new
                {
                    feedback_submit_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    student_rollno = table.Column<int>(type: "int", nullable: true),
                    feedback_id = table.Column<int>(type: "int", nullable: true),
                    feedback_group_id = table.Column<int>(type: "int", nullable: true),
                    submited_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackSubmits", x => x.feedback_submit_id);
                    table.ForeignKey(
                        name: "FK_FeedbackSubmits_FeedbackGroup_feedback_group_id",
                        column: x => x.feedback_group_id,
                        principalTable: "FeedbackGroup",
                        principalColumn: "FeedbackGroupId");
                    table.ForeignKey(
                        name: "FK_FeedbackSubmits_Feedback_feedback_id",
                        column: x => x.feedback_id,
                        principalTable: "Feedback",
                        principalColumn: "feedback_id");
                    table.ForeignKey(
                        name: "FK_FeedbackSubmits_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackAnswers",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_id = table.Column<int>(type: "int", nullable: true),
                    answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_submit_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackAnswers", x => x.answer_id);
                    table.ForeignKey(
                        name: "FK_FeedbackAnswers_FeedbackQuestions_question_id",
                        column: x => x.question_id,
                        principalTable: "FeedbackQuestions",
                        principalColumn: "question_id");
                    table.ForeignKey(
                        name: "FK_FeedbackAnswers_FeedbackSubmits_feedback_submit_id",
                        column: x => x.feedback_submit_id,
                        principalTable: "FeedbackSubmits",
                        principalColumn: "feedback_submit_id");
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
                name: "IX_CourseStudents_course_id",
                table: "CourseStudents",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_student_rollno",
                table: "CourseStudents",
                column: "student_rollno");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_course_id",
                table: "Feedback",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_feedback_type_id",
                table: "Feedback",
                column: "feedback_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_module_id",
                table: "Feedback",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_feedback_submit_id",
                table: "FeedbackAnswers",
                column: "feedback_submit_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_question_id",
                table: "FeedbackAnswers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_GroupId",
                table: "FeedbackGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_StaffId",
                table: "FeedbackGroup",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestions_feedback_type_id",
                table: "FeedbackQuestions",
                column: "feedback_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_feedback_group_id",
                table: "FeedbackSubmits",
                column: "feedback_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_feedback_id",
                table: "FeedbackSubmits",
                column: "feedback_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_student_rollno",
                table: "FeedbackSubmits",
                column: "student_rollno");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_course_id",
                table: "Modules",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_staffrole_id",
                table: "RoleAccesses",
                column: "staffrole_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccesses_student_rollno",
                table: "RoleAccesses",
                column: "student_rollno");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_staffrole_id",
                table: "Staff",
                column: "staffrole_id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_group_id",
                table: "Students",
                column: "group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseGroups");

            migrationBuilder.DropTable(
                name: "CourseStudents");

            migrationBuilder.DropTable(
                name: "FeedbackAnswers");

            migrationBuilder.DropTable(
                name: "RoleAccesses");

            migrationBuilder.DropTable(
                name: "FeedbackQuestions");

            migrationBuilder.DropTable(
                name: "FeedbackSubmits");

            migrationBuilder.DropTable(
                name: "FeedbackGroup");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "FeedbackType");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Staffroles");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
