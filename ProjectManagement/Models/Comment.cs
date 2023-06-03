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
        [Unicode(false)]
        [Required]
        [Display(Name ="Comment Text")]
        [StringLength(255, ErrorMessage = "The Comment tex field must be a maximum of 255 characters.")]
        public string CommentText { get; set; } = null!;
        [Column("posted_at", TypeName = "date")]
        [Display(Name = "Posted at")]
        public DateTime PostedAt { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("author_id")]
        [StringLength(255)]
        [Unicode(false)]
        [Display(Name ="Author")]
        public string AuthorId { get; set; } = null!;

        [ForeignKey("AuthorId")]
        [InverseProperty("Comments")]
        public virtual User Author { get; set; } = null!;
        [ForeignKey("TaskId")]
        [InverseProperty("Comments")]
        public virtual Task Task { get; set; } = null!;
    }
}
