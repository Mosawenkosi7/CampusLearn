namespace CampusLearn.Models
{
    public class LearningResource
    {
        public int ResourceId { get; set; }
        public int TutorId { get; set; }
        public string ModuleCode { get; set; } = "";
        public string Topic { get; set; } = "";
        public string Description { get; set; } = "";
        public string Discussion { get; set; } = "";
        public IFormFile? Document1 { get; set; }
        public IFormFile? Document2 { get; set; }
        public IFormFile? Document3 { get; set; }
        public string Link1 { get; set; } = "";
        public string Link2 { get; set; } = "";
        public bool AddToDiscussion { get; set; }
    }
}
