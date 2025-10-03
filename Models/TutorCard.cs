namespace CampusLearn.Models
{
    public class TutorCard : TutorProfile
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public int YearOfStudy { get; set; }
        public string Program { get; set; } = "";
        public string Campus { get; set; } = "";
        public List<string> Modules { get; set; } = new List<string>();

        public string FullName => $"{FirstName} {LastName}";
        public string Bio => TutorSummary ?? "No bio available";
        public decimal AverageGrade => AcademicAverage;
        public int YearsOfStudy => YearOfStudy;
    }
}