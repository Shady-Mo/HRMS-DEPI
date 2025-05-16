using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Migrations
{
    /// <inheritdoc />
    public partial class MakeDeleteConstraintSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
