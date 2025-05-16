using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEPI_Project.Migrations
{
    /// <inheritdoc />
    public partial class newnameRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectEmployees_AspNetUsers_EmployeeId",
                table: "ProjectEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectEmployees_Projects_ProjectId",
                table: "ProjectEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectEmployees",
                table: "ProjectEmployees");

            migrationBuilder.RenameTable(
                name: "ProjectEmployees",
                newName: "AssignedEmployees");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectEmployees_EmployeeId",
                table: "AssignedEmployees",
                newName: "IX_AssignedEmployees_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedEmployees",
                table: "AssignedEmployees",
                columns: new[] { "ProjectId", "EmployeeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedEmployees_AspNetUsers_EmployeeId",
                table: "AssignedEmployees",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedEmployees_Projects_ProjectId",
                table: "AssignedEmployees",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedEmployees_AspNetUsers_EmployeeId",
                table: "AssignedEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedEmployees_Projects_ProjectId",
                table: "AssignedEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedEmployees",
                table: "AssignedEmployees");

            migrationBuilder.RenameTable(
                name: "AssignedEmployees",
                newName: "ProjectEmployees");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedEmployees_EmployeeId",
                table: "ProjectEmployees",
                newName: "IX_ProjectEmployees_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectEmployees",
                table: "ProjectEmployees",
                columns: new[] { "ProjectId", "EmployeeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectEmployees_AspNetUsers_EmployeeId",
                table: "ProjectEmployees",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectEmployees_Projects_ProjectId",
                table: "ProjectEmployees",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
