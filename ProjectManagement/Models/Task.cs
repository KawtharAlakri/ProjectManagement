using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Task")]
    public partial class Task
    {
        public Task()
        {
            Comments = new HashSet<Comment>();
            Documents = new HashSet<Document>();
        }

        [Key]
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("Assigned_to")]
        [StringLength(255)]
        [Unicode(false)]
        public string AssignedTo { get; set; } = null!;
        [Column("task_name")]
        [StringLength(255)]
        [Unicode(false)]
        public string TaskName { get; set; } = null!;
        [Column("start_date", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("end_date", TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("Project_priject_id")]
        public int ProjectPrijectId { get; set; }
        [Column("task_status")]
        [StringLength(255)]
        [Unicode(false)]
        public string TaskStatus { get; set; } = null!;

        [ForeignKey("AssignedTo")]
        [InverseProperty("Tasks")]
        public virtual User AssignedToNavigation { get; set; } = null!;
        [ForeignKey("ProjectPrijectId")]
        [InverseProperty("Tasks")]
        public virtual Project ProjectPriject { get; set; } = null!;
        [ForeignKey("TaskStatus")]
        [InverseProperty("Tasks")]
        public virtual Status TaskStatusNavigation { get; set; } = null!;
        [InverseProperty("Task")]
        public virtual ICollection<Comment> Comments { get; set; }
        [InverseProperty("TaskTask")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}
