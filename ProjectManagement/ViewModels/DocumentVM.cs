namespace ProjectManagement.ViewModels
{
    public class DocumentVM
    {
        public string DocumentName { get; set; }
        public IFormFile uploadedFile { get; set; }
        public int TaskId { get; set; }
    }
}
