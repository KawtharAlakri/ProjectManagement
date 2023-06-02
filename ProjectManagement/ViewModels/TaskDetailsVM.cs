using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class TaskDetailsVM
    {
        public Models.Task task { get; set; }
        public List<string> selectedUsers { get; set; }
        public Project project { get; set; } 
        public IEnumerable<Comment> comments { get; set; }
        public Comment comment { get; set; }
        public Document document { get; set; }
        public IEnumerable<Document> documents { get; set; }
    }
}
