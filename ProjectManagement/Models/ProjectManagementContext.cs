using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectManagement.Models
{
    public partial class ProjectManagementContext : DbContext
    {
        public ProjectManagementContext()
        {
        }

        public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserProject> UserProjects { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ProjectManagementDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Discriminator).HasDefaultValueSql("(N'')");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).ValueGeneratedNever();

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Comment_User");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Comment_Task");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentId).ValueGeneratedNever();

                entity.HasOne(d => d.TaskTask)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.TaskTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Document_Task");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.LogId).ValueGeneratedNever();

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Log_User");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).ValueGeneratedNever();

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Notification_User");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).ValueGeneratedNever();

                entity.HasOne(d => d.ProjectManagerNavigation)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ProjectManager)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Project_User");

                entity.HasOne(d => d.ProjectStatusNavigation)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ProjectStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Project_Status");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.StatusName)
                    .HasName("Status_pk");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskId).ValueGeneratedNever();

                entity.HasOne(d => d.AssignedToNavigation)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.AssignedTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_User");

                entity.HasOne(d => d.ProjectPriject)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectPrijectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_Project");

                entity.HasOne(d => d.TaskStatusNavigation)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.TaskStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_Status");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("User_pk");
            });

            modelBuilder.Entity<UserProject>(entity =>
            {
                entity.Property(e => e.UserProjectId).ValueGeneratedNever();

                entity.HasOne(d => d.ProjectPriject)
                    .WithMany(p => p.UserProjects)
                    .HasForeignKey(d => d.ProjectPrijectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserProject_Project");

                entity.HasOne(d => d.UserUsernameNavigation)
                    .WithMany(p => p.UserProjects)
                    .HasForeignKey(d => d.UserUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserProject_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
