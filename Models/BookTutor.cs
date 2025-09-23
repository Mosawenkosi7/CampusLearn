namespace CampusLearn.Models
{
    public class BookTutor
    {
        /*  
         *  Can get this from the student class
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        */

        /* subject/module class
        public string Subject { get; set; } = string.Empty;
        */
        /* From Tutor availability
        public DateTime PreferredDate { get; set; }
        */
        public string PreferredTime { get; set; } = string.Empty;
        public int SessionDuration { get; set; }
        public string? AdditionalNotes { get; set; }
        public string? UploadedFiles { get; set; }
        public bool TermsAgreed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SessionNotes { get; set; }
    }
}
