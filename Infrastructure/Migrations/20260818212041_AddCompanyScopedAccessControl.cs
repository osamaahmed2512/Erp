using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyScopedAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRootSuperAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CompanyMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDelegatedAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "CompanyRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRoles_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPages", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "PermissionDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionDefinitions_SystemPages_SystemPageId",
                        column: x => x.SystemPageId,
                        principalTable: "SystemPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRolePermissions_CompanyRoles_CompanyRoleId",
                        column: x => x.CompanyRoleId,
                        principalTable: "CompanyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyRolePermissions_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyMembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrides_CompanyMemberships_CompanyMembershipId",
                        column: x => x.CompanyMembershipId,
                        principalTable: "CompanyMemberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionOverrides_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRolePermissions_CompanyRoleId_PermissionDefinitionId",
                table: "CompanyRolePermissions",
                columns: new[] { "CompanyRoleId", "PermissionDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRolePermissions_PermissionDefinitionId",
                table: "CompanyRolePermissions",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoles_CompanyId_NormalizedName",
                table: "CompanyRoles",
                columns: new[] { "CompanyId", "NormalizedName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_Key",
                table: "PermissionDefinitions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_SystemPageId",
                table: "PermissionDefinitions",
                column: "SystemPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPages_Key",
                table: "SystemPages",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrides_CompanyMembershipId_PermissionDefinitionId",
                table: "UserPermissionOverrides",
                columns: new[] { "CompanyMembershipId", "PermissionDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrides_PermissionDefinitionId",
                table: "UserPermissionOverrides",
                column: "PermissionDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyMembershipRoles");

            migrationBuilder.DropTable(
                name: "CompanyRolePermissions");

            migrationBuilder.DropTable(
                name: "UserPermissionOverrides");

            migrationBuilder.DropTable(
                name: "CompanyRoles");

            migrationBuilder.DropTable(
                name: "CompanyMemberships");

            migrationBuilder.DropTable(
                name: "PermissionDefinitions");

            migrationBuilder.DropTable(
                name: "SystemPages");

            migrationBuilder.DropColumn(
                name: "IsRootSuperAdmin",
                table: "AspNetUsers");
        }
    }
}
