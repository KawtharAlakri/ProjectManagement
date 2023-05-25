using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations.ProjectManagement
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    status_name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.status_name);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    username = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    created_at = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    due_date = table.Column<DateTime>(type: "date", nullable: true),
                    budget = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    description = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 400, nullable: true),
                    project_manager = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.project_id);
                    table.ForeignKey(
                        name: "FK_Project_Status",
                        column: x => x.status,
                        principalTable: "Status",
                        principalColumn: "status_name");
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    log_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    message = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    current_value = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    original_value = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    source = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    pageSource = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    log_timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    username = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.log_id);
                    table.ForeignKey(
                        name: "FK_Log_User",
                        column: x => x.username,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_text = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    recipient = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    generated_at = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notification_User",
                        column: x => x.recipient,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    assigned_to = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    task_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    created_at = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    due_date = table.Column<DateTime>(type: "date", nullable: true),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.task_id);
                    table.ForeignKey(
                        name: "FK_Task_Project",
                        column: x => x.project_id,
                        principalTable: "Project",
                        principalColumn: "project_id");
                    table.ForeignKey(
                        name: "FK_Task_Status",
                        column: x => x.status,
                        principalTable: "Status",
                        principalColumn: "status_name");
                    table.ForeignKey(
                        name: "FK_Task_User",
                        column: x => x.assigned_to,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "UserProject",
                columns: table => new
                {
                    userProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    project_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProject", x => x.userProjectId);
                    table.ForeignKey(
                        name: "FK_UserProject_Project",
                        column: x => x.project_id,
                        principalTable: "Project",
                        principalColumn: "project_id");
                    table.ForeignKey(
                        name: "FK_UserProject_User",
                        column: x => x.username,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comment_text = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    posted_at = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    task_id = table.Column<int>(type: "int", nullable: false),
                    author_id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_Comment_Task",
                        column: x => x.task_id,
                        principalTable: "Task",
                        principalColumn: "task_id");
                    table.ForeignKey(
                        name: "FK_Comment_User",
                        column: x => x.author_id,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    document_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    document_name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    document_type = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    uploaded_at = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    task_id = table.Column<int>(type: "int", nullable: false),
                    uploaded_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.document_id);
                    table.ForeignKey(
                        name: "FK_Document_Task",
                        column: x => x.task_id,
                        principalTable: "Task",
                        principalColumn: "task_id");
                    table.ForeignKey(
                        name: "FK_Document_User",
                        column: x => x.uploaded_by,
                        principalTable: "User",
                        principalColumn: "username");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_author_id",
                table: "Comment",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_task_id",
                table: "Comment",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_Document_task_id",
                table: "Document",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_Document_uploaded_by",
                table: "Document",
                column: "uploaded_by");

            migrationBuilder.CreateIndex(
                name: "IX_Log_username",
                table: "Log",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_recipient",
                table: "Notification",
                column: "recipient");

            migrationBuilder.CreateIndex(
                name: "IX_Project_status",
                table: "Project",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Task_assigned_to",
                table: "Task",
                column: "assigned_to");

            migrationBuilder.CreateIndex(
                name: "IX_Task_project_id",
                table: "Task",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_Task_status",
                table: "Task",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_project_id",
                table: "UserProject",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_username",
                table: "UserProject",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "UserProject");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Status");
        }
    }
}
