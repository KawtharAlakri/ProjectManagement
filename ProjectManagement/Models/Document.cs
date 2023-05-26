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
        [Column("document_type")]
        [StringLength(25)]
        [Unicode(false)]
        public string DocumentType { get; set; } = null!;
        [Column("uploaded_at", TypeName = "date")]
        public DateTime UploadedAt { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("uploaded_by")]
        [StringLength(255)]
        [Unicode(false)]
        public string? UploadedBy { get; set; }

        [ForeignKey("TaskId")]
        [InverseProperty("Documents")]
        public virtual Task Task { get; set; } = null!;
        [ForeignKey("UploadedBy")]
        [InverseProperty("Documents")]
        public virtual User? UploadedByNavigation { get; set; }
    }
}
