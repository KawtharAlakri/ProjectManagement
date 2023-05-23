using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("Notification")]
    public partial class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }
        [Column("notification_text")]
        [StringLength(300)]
        [Unicode(false)]
        public string NotificationText { get; set; } = null!;
        [Column("recipient")]
        [StringLength(255)]
        [Unicode(false)]
        public string Recipient { get; set; } = null!;
        [Column("is_read")]
        public bool IsRead { get; set; }
        [Column("generated_at", TypeName = "datetime")]
        public DateTime GeneratedAt { get; set; }

        [ForeignKey("Recipient")]
        [InverseProperty("Notifications")]
        public virtual User RecipientNavigation { get; set; } = null!;
    }
}
