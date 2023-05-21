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
        [StringLength(255)]
        [Unicode(false)]
        public string NotificationText { get; set; } = null!;
        [Column("recipient_id")]
        public int RecipientId { get; set; }
        [Column("is_read")]
        public bool IsRead { get; set; }
        [Column("notification_timestamp", TypeName = "datetime")]
        public DateTime NotificationTimestamp { get; set; }
        [Column("User_username")]
        [StringLength(255)]
        [Unicode(false)]
        public string UserUsername { get; set; } = null!;

        [ForeignKey("UserUsername")]
        [InverseProperty("Notifications")]
        public virtual User UserUsernameNavigation { get; set; } = null!;
    }
}
