using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmploymentAssignmentsAndLinkPositionsToDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Company_CompanyId",
                table: "Positions");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Positions",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions",
                newName: "IX_Positions_DepartmentId");

            migrationBuilder.CreateTable(
                name: "EmploymentAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentAssignments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentAssignments_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentAssignments_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentAssignments_DepartmentId",
                table: "EmploymentAssignments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentAssignments_EmployeeId_EffectiveFrom",
                table: "EmploymentAssignments",
                columns: new[] { "EmployeeId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentAssignments_ManagerId",
                table: "EmploymentAssignments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentAssignments_PositionId",
                table: "EmploymentAssignments",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Departments_DepartmentId",
                table: "Positions");

            migrationBuilder.DropTable(
                name: "EmploymentAssignments");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Positions",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_DepartmentId",
                table: "Positions",
                newName: "IX_Positions_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Company_CompanyId",
                table: "Positions",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
