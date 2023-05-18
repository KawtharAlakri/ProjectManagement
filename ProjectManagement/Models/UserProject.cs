using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("UserProject")]
    public partial class UserProject
    {
        [Key]
        [Column("userProjectId")]
        public int UserProjectId { get; set; }
        [Column("username")]
        [StringLength(255)]
        [Unicode(false)]
        public string Username { get; set; } = null!;
        [Column("User_username")]
        [StringLength(255)]
        [Unicode(false)]
        public string UserUsername { get; set; } = null!;
        [Column("Project_priject_id")]
        public int ProjectPrijectId { get; set; }

        [ForeignKey("ProjectPrijectId")]
        [InverseProperty("UserProjects")]
        public virtual Project ProjectPriject { get; set; } = null!;
        [ForeignKey("UserUsername")]
        [InverseProperty("UserProjects")]
        public virtual User UserUsernameNavigation { get; set; } = null!;
    }
}
