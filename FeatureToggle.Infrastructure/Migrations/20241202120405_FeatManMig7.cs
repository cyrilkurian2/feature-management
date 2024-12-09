using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureToggle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FeatManMig7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "featuremanagement",
                table: "Log");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "featuremanagement",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "featuremanagement",
                table: "Log",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                schema: "featuremanagement",
                table: "Log",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_User_UserId",
                schema: "featuremanagement",
                table: "Log",
                column: "UserId",
                principalSchema: "featuremanagement",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_User_UserId",
                schema: "featuremanagement",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UserId",
                schema: "featuremanagement",
                table: "Log");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                schema: "featuremanagement",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "featuremanagement",
                table: "Log",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "featuremanagement",
                table: "Log",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
