using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAcademyCQRS.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToServiceStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ServiceSteps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ServiceSteps");
        }
    }
}
