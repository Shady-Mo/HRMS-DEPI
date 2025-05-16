using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ManagedDepartmentDepartmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId",
                unique: true,
                filter: "[ManagerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ManagedDepartmentDepartmentId",
                table: "AspNetUsers",
                column: "ManagedDepartmentDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_ManagedDepartmentDepartmentId",
                table: "AspNetUsers",
                column: "ManagedDepartmentDepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_ManagedDepartmentDepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ManagedDepartmentDepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManagedDepartmentDepartmentId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
