namespace CampusLearn.Models
{
    public class User
    {
        public string PersonnelNumber { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        //full name
        // Computed property (read-only)
        public string FullName => FirstName + " " + LastName;
        public DateTime? LastLogin { get; set; }
    }
}