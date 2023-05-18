using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Log")]
    public partial class Log
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }
        [Column("log_type")]
        [StringLength(20)]
        [Unicode(false)]
        public string LogType { get; set; } = null!;
        [Column("message")]
        [StringLength(500)]
        [Unicode(false)]
        public string? Message { get; set; }
        [Column("current_value")]
        [StringLength(500)]
        [Unicode(false)]
        public string? CurrentValue { get; set; }
        [Column("original_value")]
        [StringLength(500)]
        [Unicode(false)]
        public string? OriginalValue { get; set; }
        [Column("source")]
        [StringLength(255)]
        [Unicode(false)]
        public string Source { get; set; } = null!;
        [Column("pageSource")]
        [StringLength(255)]
        [Unicode(false)]
        public string PageSource { get; set; } = null!;
        [Column("log_timestamp")]
        public byte[] LogTimestamp { get; set; } = null!;
        [Column("username")]
        [StringLength(255)]
        [Unicode(false)]
        public string Username { get; set; } = null!;

        [ForeignKey("Username")]
        [InverseProperty("Logs")]
        public virtual User UsernameNavigation { get; set; } = null!;
    }
}
