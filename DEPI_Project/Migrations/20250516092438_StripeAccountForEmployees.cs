using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Migrations
{
    /// <inheritdoc />
    public partial class StripeAccountForEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "AspNetUsers");
        }
    }
}
