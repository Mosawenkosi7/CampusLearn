namespace CampusLearn.Models
{
    // Composite class that inherits from TutorProfile and adds User + StudentProfile data
    public class TutorCard : TutorProfile
    {
        // Inherited from TutorProfile: TutorId, StudentNumber, ProfilePicture, AcademicAverage, TutorSummary, CreatedAt, IsActive
        
        // From User table
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        
        // From StudentProfile table
        public int YearOfStudy { get; set; }
        public string Program { get; set; } = "";
        public string Campus { get; set; } = "";
        
        // Computed properties for display
        public string FullName => $"{FirstName} {LastName}";
        public string Bio => TutorSummary ?? "No bio available";
        public decimal AverageGrade => AcademicAverage;
        public int YearsOfStudy => YearOfStudy;
    }
}