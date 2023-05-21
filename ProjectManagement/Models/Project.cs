using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Project")]
    public partial class Project
    {
        public Project()
        {
            Tasks = new HashSet<Task>();
            UserProjects = new HashSet<UserProject>();
        }

        [Key]
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("project_name")]
        [StringLength(255)]
        [Unicode(false)]
        public string ProjectName { get; set; } = null!;
        [Column("created_at")]
        public byte[] CreatedAt { get; set; } = null!;
        [Column("due_date", TypeName = "date")]
        public DateTime? DueDate { get; set; }
        [Column("budget", TypeName = "decimal(4, 2)")]
        public decimal? Budget { get; set; }
        [Column("description")]
        [StringLength(400)]
        [Unicode(false)]
        public string? Description { get; set; }
        [Column("project_manager")]
        [StringLength(255)]
        [Unicode(false)]
        public string ProjectManager { get; set; } = null!;
        [Column("status")]
        [StringLength(25)]
        [Unicode(false)]
        public string Status { get; set; } = null!;

        [ForeignKey("Status")]
        [InverseProperty("Projects")]
        public virtual Status StatusNavigation { get; set; } = null!;
        [InverseProperty("Project")]
        public virtual ICollection<Task> Tasks { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<UserProject> UserProjects { get; set; }
    }
}
