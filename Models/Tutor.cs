namespace CampusLearn.Models
{
    public class TutorCard
    {
        public int TutorId { get; set; }
        public decimal AverageGrade { get; set; }
        public int YearsOfStudy { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public List<string> Modules { get; set; } = new List<string>();
    }

    public class TutorQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Search { get; set; }
        public string? ModuleCode { get; set; }
        public int? YearOfStudy { get; set; }
    }

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
