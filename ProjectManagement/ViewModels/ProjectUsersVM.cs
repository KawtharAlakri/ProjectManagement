using ProjectManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.ViewModels
{
    public class ProjectUsersVM
    {
        public Project project { get; set; }
        [Required(ErrorMessage ="You must add at least one member to the project")]
        public List<string> selectedUsers { get; set; }
        public IEnumerable<User> allUsers { get; set; }
    }
}
