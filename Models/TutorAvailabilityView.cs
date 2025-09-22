namespace CampusLearn.Models
{
    // Composite class that inherits from TutorAvailability and adds display properties
    public class TutorAvailabilityView : TutorAvailability
    {
        // Inherited from TutorAvailability: TutorAvailabilityId, TutorId, ModuleCode, Available, Location, IsActive, IsBooked
        
        // Computed properties for display only - no duplicate fields
        public int AvailabilityId => TutorAvailabilityId;
        public string Module => ModuleCode;
        public DateTime AvailableDate => Available.Date;
        public TimeSpan StartTime => Available.TimeOfDay;
        public TimeSpan EndTime => Available.AddHours(1).TimeOfDay; // Assuming 1-hour sessions
    }
}