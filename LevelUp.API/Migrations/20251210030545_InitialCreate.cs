using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    password_hash = table.Column<string>(
                        type: "nvarchar(512)",
                        maxLength: 512,
                        nullable: false
                    ),
                    role = table.Column<int>(type: "int", nullable: false),
                    otp = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    is_active = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    first_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    last_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    position_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_idle = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                    table.ForeignKey(
                        name: "FK_employees_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_employees_positions_position_id",
                        column: x => x.position_id,
                        principalTable: "positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    description = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    estimated_days = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),
                    created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.id);
                    table.ForeignKey(
                        name: "FK_modules_employees_created_by",
                        column: x => x.created_by,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "enrollments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    module_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    completed_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    current_progress = table.Column<int>(
                        type: "int",
                        nullable: false,
                        defaultValue: 0
                    ),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "FK_enrollments_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_enrollments_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "module_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    module_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    order_index = table.Column<int>(type: "int", nullable: false),
                    descriptions = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    url = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    is_final_submission = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_module_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_module_items_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_submissions_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "enrollment_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    module_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_completed = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),
                    feedback = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    evidence_url = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollment_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_enrollment_items_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_enrollment_items_module_items_module_item_id",
                        column: x => x.module_item_id,
                        principalTable: "module_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_employees_account_id",
                table: "employees",
                column: "account_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_employees_position_id",
                table: "employees",
                column: "position_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_items_enrollment_id",
                table: "enrollment_items",
                column: "enrollment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_items_module_item_id",
                table: "enrollment_items",
                column: "module_item_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_account_id",
                table: "enrollments",
                column: "account_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_module_id",
                table: "enrollments",
                column: "module_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_module_items_module_id",
                table: "module_items",
                column: "module_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_modules_created_by",
                table: "modules",
                column: "created_by"
            );

            migrationBuilder.CreateIndex(
                name: "IX_submissions_enrollment_id",
                table: "submissions",
                column: "enrollment_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "enrollment_items");

            migrationBuilder.DropTable(name: "submissions");

            migrationBuilder.DropTable(name: "module_items");

            migrationBuilder.DropTable(name: "enrollments");

            migrationBuilder.DropTable(name: "modules");

            migrationBuilder.DropTable(name: "employees");

            migrationBuilder.DropTable(name: "accounts");

            migrationBuilder.DropTable(name: "positions");
        }
    }
}
