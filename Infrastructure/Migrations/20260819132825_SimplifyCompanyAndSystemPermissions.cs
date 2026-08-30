using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCompanyAndSystemPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionOverrides_CompanyMemberships_CompanyMembershipId",
                table: "UserPermissionOverrides");

            migrationBuilder.AddColumn<int>(
                name: "Audience",
                table: "SystemPages",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT UserId
                    FROM CompanyMemberships
                    GROUP BY UserId
                    HAVING COUNT(DISTINCT CompanyId) > 1
                )
                    THROW 51000, 'Permission migration stopped: a user is assigned to more than one company membership.', 1;

                IF EXISTS (
                    SELECT OwnerId
                    FROM Company
                    GROUP BY OwnerId
                    HAVING COUNT(*) > 1
                )
                    THROW 51001, 'Permission migration stopped: a company owner owns more than one company.', 1;

                IF EXISTS (
                    SELECT UserId
                    FROM Employees
                    GROUP BY UserId
                    HAVING COUNT(DISTINCT CompanyId) > 1
                )
                    THROW 51002, 'Permission migration stopped: an employee account belongs to more than one company.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM CompanyMemberships membership
                    INNER JOIN Employees employee ON employee.UserId = membership.UserId
                    WHERE employee.CompanyId <> membership.CompanyId
                )
                    THROW 51003, 'Permission migration stopped: employee and membership companies conflict.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM CompanyMemberships membership
                    INNER JOIN Company company ON company.OwnerId = membership.UserId
                    WHERE company.Id <> membership.CompanyId
                )
                    THROW 51004, 'Permission migration stopped: owner and membership companies conflict.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM Employees employee
                    INNER JOIN Company company ON company.OwnerId = employee.UserId
                    WHERE company.Id <> employee.CompanyId
                )
                    THROW 51005, 'Permission migration stopped: owner and employee companies conflict.', 1;

                UPDATE appUser
                SET CompanyId = membership.CompanyId,
                    AccountType = 1
                FROM AspNetUsers appUser
                INNER JOIN CompanyMemberships membership ON membership.UserId = appUser.Id;

                UPDATE appUser
                SET CompanyId = employee.CompanyId,
                    AccountType = 1
                FROM AspNetUsers appUser
                INNER JOIN Employees employee ON employee.UserId = appUser.Id
                WHERE appUser.CompanyId IS NULL;

                UPDATE appUser
                SET CompanyId = company.Id,
                    AccountType = 1
                FROM AspNetUsers appUser
                INNER JOIN Company company ON company.OwnerId = appUser.Id
                WHERE appUser.CompanyId IS NULL;

                UPDATE AspNetUsers
                SET AccountType = 2,
                    CompanyId = NULL
                WHERE CompanyId IS NULL;

                DELETE FROM UserPermissionOverrides WHERE Effect <> 2;

                UPDATE permissionOverride
                SET CompanyMembershipId = membership.UserId
                FROM UserPermissionOverrides permissionOverride
                INNER JOIN CompanyMemberships membership
                    ON membership.Id = permissionOverride.CompanyMembershipId;
                """);

            migrationBuilder.RenameColumn(
                name: "CompanyMembershipId",
                table: "UserPermissionOverrides",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionOverrides_CompanyMembershipId_PermissionDefinitionId",
                table: "UserPermissionOverrides",
                newName: "IX_UserPermissionOverrides_UserId_PermissionDefinitionId");

            migrationBuilder.CreateTable(
                name: "CompanyUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_CompanyRoles_CompanyRoleId",
                        column: x => x.CompanyRoleId,
                        principalTable: "CompanyRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsProtected = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemRolePermissions_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemRolePermissions_SystemRoles_SystemRoleId",
                        column: x => x.SystemRoleId,
                        principalTable: "SystemRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemUserRoles_SystemRoles_SystemRoleId",
                        column: x => x.SystemRoleId,
                        principalTable: "SystemRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_CompanyRoleId",
                table: "CompanyUserRoles",
                column: "CompanyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_UserId_CompanyRoleId",
                table: "CompanyUserRoles",
                columns: new[] { "UserId", "CompanyRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemRolePermissions_PermissionDefinitionId",
                table: "SystemRolePermissions",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRolePermissions_SystemRoleId_PermissionDefinitionId",
                table: "SystemRolePermissions",
                columns: new[] { "SystemRoleId", "PermissionDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoles_NormalizedName",
                table: "SystemRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRoles_SystemRoleId",
                table: "SystemUserRoles",
                column: "SystemRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRoles_UserId_SystemRoleId",
                table: "SystemUserRoles",
                columns: new[] { "UserId", "SystemRoleId" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO CompanyUserRoles (Id, UserId, CompanyRoleId, CreatedAt, UpdatedAt)
                SELECT membershipRole.Id,
                       membership.UserId,
                       membershipRole.CompanyRoleId,
                       membershipRole.CreatedAt,
                       membershipRole.UpdatedAt
                FROM CompanyMembershipRoles membershipRole
                INNER JOIN CompanyMemberships membership
                    ON membership.Id = membershipRole.CompanyMembershipId;
                """);

            migrationBuilder.DropTable(
                name: "CompanyMembershipRoles");

            migrationBuilder.DropTable(
                name: "CompanyMemberships");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionOverrides_AspNetUsers_UserId",
                table: "UserPermissionOverrides",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionOverrides_AspNetUsers_UserId",
                table: "UserPermissionOverrides");

            migrationBuilder.DropTable(
                name: "CompanyUserRoles");

            migrationBuilder.DropTable(
                name: "SystemRolePermissions");

            migrationBuilder.DropTable(
                name: "SystemUserRoles");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audience",
                table: "SystemPages");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserPermissionOverrides",
                newName: "CompanyMembershipId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionOverrides_UserId_PermissionDefinitionId",
                table: "UserPermissionOverrides",
                newName: "IX_UserPermissionOverrides_CompanyMembershipId_PermissionDefinitionId");

            migrationBuilder.CreateTable(
                name: "CompanyMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelegatedAdmin = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyMemberships_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyMembershipRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyMembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMembershipRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyMembershipRoles_CompanyMemberships_CompanyMembershipId",
                        column: x => x.CompanyMembershipId,
                        principalTable: "CompanyMemberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyMembershipRoles_CompanyRoles_CompanyRoleId",
                        column: x => x.CompanyRoleId,
                        principalTable: "CompanyRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembershipRoles_CompanyMembershipId_CompanyRoleId",
                table: "CompanyMembershipRoles",
                columns: new[] { "CompanyMembershipId", "CompanyRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembershipRoles_CompanyRoleId",
                table: "CompanyMembershipRoles",
                column: "CompanyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_CompanyId_UserId",
                table: "CompanyMemberships",
                columns: new[] { "CompanyId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_UserId",
                table: "CompanyMemberships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionOverrides_CompanyMemberships_CompanyMembershipId",
                table: "UserPermissionOverrides",
                column: "CompanyMembershipId",
                principalTable: "CompanyMemberships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
