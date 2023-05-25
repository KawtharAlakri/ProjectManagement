﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectManagement.Models;

#nullable disable

namespace ProjectManagement.Migrations.ProjectManagement
{
    [DbContext(typeof(ProjectManagementContext))]
    partial class ProjectManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ProjectManagement.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"), 1L, 1);

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("author_id");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("comment_text");

                    b.Property<byte[]>("PostedAt")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("posted_at");

                    b.Property<int>("TaskId")
                        .HasColumnType("int")
                        .HasColumnName("task_id");

                    b.HasKey("CommentId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TaskId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("ProjectManagement.Models.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("document_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"), 1L, 1);

                    b.Property<string>("DocumentName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("document_name");

                    b.Property<string>("DocumentType")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("document_type");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("file_path");

                    b.Property<int>("TaskId")
                        .HasColumnType("int")
                        .HasColumnName("task_id");

                    b.Property<byte[]>("UploadedAt")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("uploaded_at");

                    b.Property<string>("UploadedBy")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("uploaded_by");

                    b.HasKey("DocumentId");

                    b.HasIndex("TaskId");

                    b.HasIndex("UploadedBy");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("ProjectManagement.Models.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("log_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"), 1L, 1);

                    b.Property<string>("CurrentValue")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("current_value");

                    b.Property<byte[]>("LogTimestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("log_timestamp");

                    b.Property<string>("LogType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("log_type");

                    b.Property<string>("Message")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("message");

                    b.Property<string>("OriginalValue")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("original_value");

                    b.Property<string>("PageSource")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("pageSource");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("source");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("username");

                    b.HasKey("LogId");

                    b.HasIndex("Username");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("ProjectManagement.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("notification_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"), 1L, 1);

                    b.Property<byte[]>("GeneratedAt")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("generated_at");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit")
                        .HasColumnName("is_read");

                    b.Property<string>("NotificationText")
                        .IsRequired()
                        .HasMaxLength(300)
                        .IsUnicode(false)
                        .HasColumnType("varchar(300)")
                        .HasColumnName("notification_text");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("recipient");

                    b.HasKey("NotificationId");

                    b.HasIndex("Recipient");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("ProjectManagement.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("project_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"), 1L, 1);

                    b.Property<decimal?>("Budget")
                        .HasColumnType("decimal(4,2)")
                        .HasColumnName("budget");

                    b.Property<byte[]>("CreatedAt")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasMaxLength(400)
                        .IsUnicode(false)
                        .HasColumnType("varchar(400)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("date")
                        .HasColumnName("due_date");

                    b.Property<string>("ProjectManager")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("project_manager");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("project_name");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("status");

                    b.HasKey("ProjectId");

                    b.HasIndex("Status");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("ProjectManagement.Models.Status", b =>
                {
                    b.Property<string>("StatusName")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("status_name");

                    b.HasKey("StatusName");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("ProjectManagement.Models.Task", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("task_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"), 1L, 1);

                    b.Property<string>("AssignedTo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("assigned_to");

                    b.Property<byte[]>("CreatedAt")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("date")
                        .HasColumnName("due_date");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int")
                        .HasColumnName("project_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("status");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("task_name");

                    b.HasKey("TaskId");

                    b.HasIndex("AssignedTo");

                    b.HasIndex("ProjectId");

                    b.HasIndex("Status");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("ProjectManagement.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("username");

                    b.HasKey("Username");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ProjectManagement.Models.UserProject", b =>
                {
                    b.Property<int>("UserProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("userProjectId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserProjectId"), 1L, 1);

                    b.Property<int>("ProjectId")
                        .HasColumnType("int")
                        .HasColumnName("project_id");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("username");

                    b.HasKey("UserProjectId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("Username");

                    b.ToTable("UserProject");
                });

            modelBuilder.Entity("ProjectManagement.Models.Comment", b =>
                {
                    b.HasOne("ProjectManagement.Models.User", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .IsRequired()
                        .HasConstraintName("FK_Comment_User");

                    b.HasOne("ProjectManagement.Models.Task", "Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .IsRequired()
                        .HasConstraintName("FK_Comment_Task");

                    b.Navigation("Author");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("ProjectManagement.Models.Document", b =>
                {
                    b.HasOne("ProjectManagement.Models.Task", "Task")
                        .WithMany("Documents")
                        .HasForeignKey("TaskId")
                        .IsRequired()
                        .HasConstraintName("FK_Document_Task");

                    b.HasOne("ProjectManagement.Models.User", "UploadedByNavigation")
                        .WithMany("Documents")
                        .HasForeignKey("UploadedBy")
                        .HasConstraintName("FK_Document_User");

                    b.Navigation("Task");

                    b.Navigation("UploadedByNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.Log", b =>
                {
                    b.HasOne("ProjectManagement.Models.User", "UsernameNavigation")
                        .WithMany("Logs")
                        .HasForeignKey("Username")
                        .IsRequired()
                        .HasConstraintName("FK_Log_User");

                    b.Navigation("UsernameNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.Notification", b =>
                {
                    b.HasOne("ProjectManagement.Models.User", "RecipientNavigation")
                        .WithMany("Notifications")
                        .HasForeignKey("Recipient")
                        .IsRequired()
                        .HasConstraintName("FK_Notification_User");

                    b.Navigation("RecipientNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.Project", b =>
                {
                    b.HasOne("ProjectManagement.Models.Status", "StatusNavigation")
                        .WithMany("Projects")
                        .HasForeignKey("Status")
                        .IsRequired()
                        .HasConstraintName("FK_Project_Status");

                    b.Navigation("StatusNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.Task", b =>
                {
                    b.HasOne("ProjectManagement.Models.User", "AssignedToNavigation")
                        .WithMany("Tasks")
                        .HasForeignKey("AssignedTo")
                        .IsRequired()
                        .HasConstraintName("FK_Task_User");

                    b.HasOne("ProjectManagement.Models.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("FK_Task_Project");

                    b.HasOne("ProjectManagement.Models.Status", "StatusNavigation")
                        .WithMany("Tasks")
                        .HasForeignKey("Status")
                        .IsRequired()
                        .HasConstraintName("FK_Task_Status");

                    b.Navigation("AssignedToNavigation");

                    b.Navigation("Project");

                    b.Navigation("StatusNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.UserProject", b =>
                {
                    b.HasOne("ProjectManagement.Models.Project", "Project")
                        .WithMany("UserProjects")
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("FK_UserProject_Project");

                    b.HasOne("ProjectManagement.Models.User", "UsernameNavigation")
                        .WithMany("UserProjects")
                        .HasForeignKey("Username")
                        .IsRequired()
                        .HasConstraintName("FK_UserProject_User");

                    b.Navigation("Project");

                    b.Navigation("UsernameNavigation");
                });

            modelBuilder.Entity("ProjectManagement.Models.Project", b =>
                {
                    b.Navigation("Tasks");

                    b.Navigation("UserProjects");
                });

            modelBuilder.Entity("ProjectManagement.Models.Status", b =>
                {
                    b.Navigation("Projects");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("ProjectManagement.Models.Task", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Documents");
                });

            modelBuilder.Entity("ProjectManagement.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Documents");

                    b.Navigation("Logs");

                    b.Navigation("Notifications");

                    b.Navigation("Tasks");

                    b.Navigation("UserProjects");
                });
#pragma warning restore 612, 618
        }
    }
}
