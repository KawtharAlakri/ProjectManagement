using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class ProjectTaskVM
    {
        public Models.Task task { get; set; }
        public List<string> selectedUsers { get; set; }
        public Project project { get; set; } 
    }
}
