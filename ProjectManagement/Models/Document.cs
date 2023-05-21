using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Document")]
    public partial class Document
    {
        [Key]
        [Column("document_id")]
        public int DocumentId { get; set; }
        [Column("document_name")]
        [StringLength(255)]
        [Unicode(false)]
        public string DocumentName { get; set; } = null!;
        [Column("file_path")]
        [StringLength(255)]
        [Unicode(false)]
        public string FilePath { get; set; } = null!;
        [Column("document_timestamp", TypeName = "datetime")]
        public DateTime DocumentTimestamp { get; set; }
        [Column("Task_task_id")]
        public int TaskTaskId { get; set; }
        [Column("document_type")]
        [StringLength(255)]
        [Unicode(false)]
        public string DocumentType { get; set; } = null!;

        [ForeignKey("TaskTaskId")]
        [InverseProperty("Documents")]
        public virtual Task TaskTask { get; set; } = null!;
    }
}
