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
        [StringLength(255, ErrorMessage = "The Project Name field must be a maximum of 255 characters.")]
        [Unicode(false)]
        [Display (Name = "Project Name")]
        [Required (ErrorMessage = "The Project Name field must be filled.")]
        public string ProjectName { get; set; } = null!;
        [Column("created_at", TypeName = "date")]
        [Display(Name = "Created At")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        [Column("due_date", TypeName = "date")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        [Column("budget", TypeName = "decimal(8, 2)")]
        public decimal? Budget { get; set; }
        [Column("description")]
        [StringLength(400, ErrorMessage = "The Description field must be a maximum of 400 characters.")]
        [Unicode(false)]
        [Required]
        public string? Description { get; set; }
        [Column("project_manager")]
        [StringLength(255)]
        [Unicode(false)]
        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; } = null!;
        [Column("status")]
        [StringLength(25)]
        [Unicode(false)]
        public string Status { get; set; } = null!;

        [ForeignKey("ProjectManager")]
        [InverseProperty("Projects")]
        [Display(Name = "Project Manager")]
        public virtual User ProjectManagerNavigation { get; set; } = null!;
        [ForeignKey("Status")]
        [InverseProperty("Projects")]
        [Display(Name = "Status")]
        public virtual Status StatusNavigation { get; set; } = null!;
        [InverseProperty("Project")]
        public virtual ICollection<Task>? Tasks { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<UserProject>? UserProjects { get; set; }

    }
}
