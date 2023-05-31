using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Documents = new HashSet<Document>();
            Logs = new HashSet<Log>();
            Notifications = new HashSet<Notification>();
            Projects = new HashSet<Project>();
            Tasks = new HashSet<Task>();
            UserProjects = new HashSet<UserProject>();
        }

        [Key]
        [Column("username")]
        [StringLength(255)]
        [Unicode(false)]
        public string Username { get; set; } = null!;

        [InverseProperty("Author")]
        public virtual ICollection<Comment>? Comments { get; set; }
        [InverseProperty("UploadedByNavigation")]
        public virtual ICollection<Document>? Documents { get; set; }
        [InverseProperty("UsernameNavigation")]
        public virtual ICollection<Log>? Logs { get; set; }
        [InverseProperty("RecipientNavigation")]
        public virtual ICollection<Notification>? Notifications { get; set; }
        [InverseProperty("ProjectManagerNavigation")]
        public virtual ICollection<Project>? Projects { get; set; }
        [InverseProperty("AssignedToNavigation")]
        public virtual ICollection<Task>? Tasks { get; set; }
        [InverseProperty("UsernameNavigation")]
        public virtual ICollection<UserProject>? UserProjects { get; set; }
    }
}
