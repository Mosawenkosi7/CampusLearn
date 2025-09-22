namespace CampusLearn.Models
{
    // Composite class for tutor with availability count - inherits from TutorCard
    public class TutorWithAvailability : TutorCard
    {
        // Inherited from TutorCard: All TutorProfile + User + StudentProfile fields and computed properties
        
        // Additional properties for availability management
        public int AvailableSlots { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public List<TutorAvailabilityView> Availabilities { get; set; } = new List<TutorAvailabilityView>();
    }
}
