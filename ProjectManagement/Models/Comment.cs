using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Comment")]
    public partial class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }
        [Column("comment_text")]
        [StringLength(255)]
        [Unicode(false)]
        public string CommentText { get; set; } = null!;
        [Column("posted_at")]
        public byte[] PostedAt { get; set; } = null!;
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("author_id")]
        [StringLength(255)]
        [Unicode(false)]
        public string AuthorId { get; set; } = null!;

        [ForeignKey("AuthorId")]
        [InverseProperty("Comments")]
        public virtual User Author { get; set; } = null!;
        [ForeignKey("TaskId")]
        [InverseProperty("Comments")]
        public virtual Task Task { get; set; } = null!;
    }
}
