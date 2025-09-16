using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class feedbackSubmit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "feedback_group_id",
                table: "FeedbackSubmits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmits_feedback_group_id",
                table: "FeedbackSubmits",
                column: "feedback_group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackSubmits_FeedbackGroup_feedback_group_id",
                table: "FeedbackSubmits",
                column: "feedback_group_id",
                principalTable: "FeedbackGroup",
                principalColumn: "FeedbackGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackSubmits_FeedbackGroup_feedback_group_id",
                table: "FeedbackSubmits");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackSubmits_feedback_group_id",
                table: "FeedbackSubmits");

            migrationBuilder.DropColumn(
                name: "feedback_group_id",
                table: "FeedbackSubmits");
        }
    }
}
