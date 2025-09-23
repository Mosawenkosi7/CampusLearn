namespace CampusLearn.Models
{
    public class TutorAvailability
    {
        public int TutorAvailabilityId { get; set; }
        public int TutorId { get; set; }
        public string ModuleCode { get; set; } = "";
        public DateTime Available { get; set; }
        public string Location { get; set; } = "";
        public bool IsActive { get; set; }
        // This property is set from JOIN with Booking table
        public bool IsBooked { get; set; }
    }
}