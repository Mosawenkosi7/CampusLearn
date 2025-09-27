namespace CampusLearn.Models
{
    public class TutorProfile
    {
        public int TutorId { get; set; }
        public string StudentNumber { get; set; } = "";
        public string? ProfilePicture { get; set; }
        public decimal AcademicAverage { get; set; }
        public string? TutorSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}