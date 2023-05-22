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
        [Column("project_id")]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("UserProjects")]
        public virtual Project Project { get; set; } = null!;
        [ForeignKey("Username")]
        [InverseProperty("UserProjects")]
        public virtual User UsernameNavigation { get; set; } = null!;
    }
}
