using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialMGR : Migration
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
                name: "Modules",
                columns: table => new
                {
                    module_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    module_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.module_id);
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
                name: "FeedbackQuestions",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    question_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackQuestions", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_FeedbackQuestions_FeedbackType_feedback_type_id",
                        column: x => x.feedback_type_id,
                        principalTable: "FeedbackType",
                        principalColumn: "feedback_type_id",
                        onDelete: ReferentialAction.Cascade);
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
                    group_id = table.Column<int>(type: "int", nullable: false),
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
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    staff_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    staffrole_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    profile_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    login_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_Staff_Groups_group_id",
                        column: x => x.group_id,
                        principalTable: "Groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staff_Staffroles_staffrole_id",
                        column: x => x.staffrole_id,
                        principalTable: "Staffroles",
                        principalColumn: "staffrole_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    feedback_type_id = table.Column<int>(type: "int", nullable: false),
                    staff_id = table.Column<int>(type: "int", nullable: false),
                    session = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_FeedbackType_feedback_type_id",
                        column: x => x.feedback_type_id,
                        principalTable: "FeedbackType",
                        principalColumn: "feedback_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Modules_module_id",
                        column: x => x.module_id,
                        principalTable: "Modules",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "Staff",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackSubmits",
                columns: table => new
                {
                    feedback_submit_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    student_rollno = table.Column<int>(type: "int", nullable: false),
                    feedback_id = table.Column<int>(type: "int", nullable: false),
                    submited_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackSubmits", x => x.feedback_submit_id);
                    table.ForeignKey(
                        name: "FK_FeedbackSubmits_Feedbacks_feedback_id",
                        column: x => x.feedback_id,
                        principalTable: "Feedbacks",
                        principalColumn: "feedback_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackSubmits_Students_student_rollno",
                        column: x => x.student_rollno,
                        principalTable: "Students",
                        principalColumn: "student_rollno",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackAnswers",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback_submit_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackAnswers", x => x.answer_id);
                    table.ForeignKey(
                        name: "FK_FeedbackAnswers_FeedbackQuestions_question_id",
                        column: x => x.question_id,
                        principalTable: "FeedbackQuestions",
                        principalColumn: "question_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackAnswers_FeedbackSubmits_feedback_submit_id",
                        column: x => x.feedback_submit_id,
                        principalTable: "FeedbackSubmits",
                        principalColumn: "feedback_submit_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_feedback_submit_id",
                table: "FeedbackAnswers",
                column: "feedback_submit_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_question_id",
                table: "FeedbackAnswers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestions_feedback_type_id",
                table: "FeedbackQuestions",
                column: "feedback_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_course_id",
                table: "Feedbacks",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_feedback_type_id",
                table: "Feedbacks",
                column: "feedback_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_module_id",
                table: "Feedbacks",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_staff_id",
                table: "Feedbacks",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_feedback_id",
                table: "FeedbackSubmits",
                column: "feedback_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_student_rollno",
                table: "FeedbackSubmits",
                column: "student_rollno");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_group_id",
                table: "Staff",
                column: "group_id");

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
                name: "FeedbackAnswers");

            migrationBuilder.DropTable(
                name: "FeedbackQuestions");

            migrationBuilder.DropTable(
                name: "FeedbackSubmits");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "FeedbackType");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Staffroles");
        }
    }
}
