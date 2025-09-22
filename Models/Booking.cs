namespace CampusLearn.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string StudentNumber { get; set; } = "";
        public int TutorAvailabilityId { get; set; }
        public string Location { get; set; } = "";
        public string? BookingSummary { get; set; }
        public DateTime DateBooked { get; set; }
        public string Status { get; set; } = "";
    }
}