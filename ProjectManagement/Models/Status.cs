using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Status")]
    public partial class Status
    {
        public Status()
        {
            Projects = new HashSet<Project>();
            Tasks = new HashSet<Task>();
        }

        [Key]
        [Column("status_name")]
        [StringLength(255)]
        [Unicode(false)]
        public string StatusName { get; set; } = null!;

        [InverseProperty("ProjectStatusNavigation")]
        public virtual ICollection<Project> Projects { get; set; }
        [InverseProperty("TaskStatusNavigation")]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
