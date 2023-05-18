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
        [Column("assigned_to")]
        [StringLength(255)]
        [Unicode(false)]
        public string AssignedTo { get; set; } = null!;
        [Column("task_name")]
        [StringLength(255)]
        [Unicode(false)]
        public string TaskName { get; set; } = null!;
        [Column("created_at")]
        public byte[] CreatedAt { get; set; } = null!;
        [Column("due_date", TypeName = "date")]
        public DateTime? DueDate { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("status")]
        [StringLength(25)]
        [Unicode(false)]
        public string Status { get; set; } = null!;

        [ForeignKey("AssignedTo")]
        [InverseProperty("Tasks")]
        public virtual User AssignedToNavigation { get; set; } = null!;
        [ForeignKey("ProjectId")]
        [InverseProperty("Tasks")]
        public virtual Project Project { get; set; } = null!;
        [ForeignKey("Status")]
        [InverseProperty("Tasks")]
        public virtual Status StatusNavigation { get; set; } = null!;
        [InverseProperty("Task")]
        public virtual ICollection<Comment> Comments { get; set; }
        [InverseProperty("Task")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}
