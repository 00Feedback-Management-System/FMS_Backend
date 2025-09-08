using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class feedbackgroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Groups_group_id",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Staff_staff_id",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_group_id",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_staff_id",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "staff_id",
                table: "Feedback",
                newName: "FeedbackGroup_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup",
                column: "FeedbackId",
                unique: true,
                filter: "[FeedbackId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_GroupId",
                table: "FeedbackGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_StaffId",
                table: "FeedbackGroup",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackGroup");

            migrationBuilder.RenameColumn(
                name: "FeedbackGroup_id",
                table: "Feedback",
                newName: "staff_id");

            migrationBuilder.AddColumn<int>(
                name: "group_id",
                table: "Feedback",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_group_id",
                table: "Feedback",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_staff_id",
                table: "Feedback",
                column: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Groups_group_id",
                table: "Feedback",
                column: "group_id",
                principalTable: "Groups",
                principalColumn: "group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Staff_staff_id",
                table: "Feedback",
                column: "staff_id",
                principalTable: "Staff",
                principalColumn: "staff_id");
        }
    }
}
