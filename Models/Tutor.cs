namespace CampusLearn.Models
{
    /// <summary>
    /// Composite class for tutor with availability count - inherits from TutorCard
    /// </summary>
    public class TutorWithAvailability : TutorCard
    {
        // Inherited from TutorCard: All TutorProfile + User + StudentProfile fields and computed properties
        
        // Additional properties for availability management
        public int AvailableSlots { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public List<TutorAvailabilityView> Availabilities { get; set; } = new List<TutorAvailabilityView>();
    }

    /// <summary>
    /// Query parameters for tutor search and filtering
    /// </summary>
    public class TutorQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Search { get; set; }
        public string? ModuleCode { get; set; }
        public int? YearOfStudy { get; set; }
    }

    /// <summary>
    /// Generic paged result container
    /// </summary>
    /// <typeparam name="T">Type of items in the result</typeparam>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize <= 0)
                {
                    return 0;
                }
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
}
