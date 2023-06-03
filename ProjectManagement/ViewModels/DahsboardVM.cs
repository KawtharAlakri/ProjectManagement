using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class DahsboardVM
    {
        public IEnumerable<Models.Task> Tasks { get; set; }
        public IEnumerable<Project> projects { get; set; }
    }
}
