namespace CampusLearn.Models
{
    public class StudentProfile
    {
        public string StudentNumber { get; set; } = "";
        public int YearOfStudy { get; set; }
        public string Program { get; set; } = "";
        public string Campus { get; set; } = "";
        public DateTime EnrollmentDate { get; set; }
    }
}