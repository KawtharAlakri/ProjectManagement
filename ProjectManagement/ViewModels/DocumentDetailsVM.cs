using ProjectManagement.Models;

namespace ProjectManagement.ViewModels
{
    public class DocumentDetailsVM
    {
        public IEnumerable<Document>? documents { get; set; }
        public Document document { get; set; }
        public Models.Task task { get; set; }
        public Project? project { get; set; }
    }
}
