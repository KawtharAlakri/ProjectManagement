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
        [Column("message")]
        [StringLength(255)]
        [Unicode(false)]
        public string? Message { get; set; }
        [Column("current_value")]
        [StringLength(255)]
        [Unicode(false)]
        public string? CurrentValue { get; set; }
        [Column("original_value")]
        [StringLength(255)]
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
        [Column("log_timestamp", TypeName = "time(4)")]
        public TimeSpan LogTimestamp { get; set; }
        [Column("User_username")]
        [StringLength(255)]
        [Unicode(false)]
        public string UserUsername { get; set; } = null!;
        [Column("log_type", TypeName = "text")]
        public string LogType { get; set; } = null!;

        [ForeignKey("UserUsername")]
        [InverseProperty("Logs")]
        public virtual User UserUsernameNavigation { get; set; } = null!;
    }
}
