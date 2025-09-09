using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feedback_System.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup");

            migrationBuilder.DropColumn(
                name: "FeedbackGroup_id",
                table: "Feedback");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup",
                column: "FeedbackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup");

            migrationBuilder.AddColumn<int>(
                name: "FeedbackGroup_id",
                table: "Feedback",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_FeedbackId",
                table: "FeedbackGroup",
                column: "FeedbackId",
                unique: true,
                filter: "[FeedbackId] IS NOT NULL");
        }
    }
}
