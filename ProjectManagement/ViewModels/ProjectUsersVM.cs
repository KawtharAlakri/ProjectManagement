using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class ProjectUsersVM
    {
        public Project project { get; set; }
        public List<string> selectedUsers { get; set; }
        public IEnumerable<User> allUsers { get; set; }

    }
}
