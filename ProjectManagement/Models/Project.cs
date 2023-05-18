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
        [Column("start_date", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("end_date", TypeName = "date")]
        public DateTime? EndDate { get; set; }
        [Column("budget", TypeName = "decimal(4, 2)")]
        public decimal? Budget { get; set; }
        [Column("project_description", TypeName = "text")]
        public string ProjectDescription { get; set; } = null!;
        [Column("projectManager")]
        [StringLength(255)]
        [Unicode(false)]
        public string ProjectManager { get; set; } = null!;
        [Column("project_status")]
        [StringLength(255)]
        [Unicode(false)]
        public string ProjectStatus { get; set; } = null!;

        [ForeignKey("ProjectManager")]
        [InverseProperty("Projects")]
        public virtual User ProjectManagerNavigation { get; set; } = null!;
        [ForeignKey("ProjectStatus")]
        [InverseProperty("Projects")]
        public virtual Status ProjectStatusNavigation { get; set; } = null!;
        [InverseProperty("ProjectPriject")]
        public virtual ICollection<Task> Tasks { get; set; }
        [InverseProperty("ProjectPriject")]
        public virtual ICollection<UserProject> UserProjects { get; set; }
    }
}
