using CampusLearn.Repositories;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    /// <summary>
    /// Service layer for tutor-related business logic
    /// </summary>
    public class TutorService
    {
        private readonly TutorRepository _tutorRepository;
        
        public TutorService(TutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        /// <summary>
        /// Gets top tutors with academic average >= 75%
        /// </summary>
        /// <returns>List of qualified tutors</returns>
        public List<TutorCard> GetTopTutors()
        {
            var tutors = _tutorRepository.GetTopTutors();
            // Filter tutors with academic average >= 75%
            return tutors.Where(tutor => tutor.AverageGrade >= 75).ToList();
        }

        /// <summary>
        /// Gets a specific tutor's profile
        /// </summary>
        /// <param name="tutorId">The tutor's ID</param>
        /// <returns>TutorCard or null if not found</returns>
        public TutorCard? GetTutorProfile(int tutorId)
        {
            return _tutorRepository.GetTutorProfile(tutorId);
        }

        /// <summary>
        /// Gets available time slots for a tutor (future, unbooked, non-overlapping)
        /// </summary>
        /// <param name="tutorId">The tutor's ID</param>
        /// <returns>List of available time slots</returns>
        public List<TutorAvailabilityView> GetTutorAvailability(int tutorId)
        {
            var availabilities = _tutorRepository.GetTutorAvailability(tutorId);
            // Additional filtering for future dates and unbooked slots
            return availabilities.Where(a => a.Available >= DateTime.Now && !a.IsBooked).ToList();
        }

        /// <summary>
        /// Gets a specific availability by ID
        /// </summary>
        /// <param name="availabilityId">The availability ID</param>
        /// <returns>TutorAvailabilityView or null if not found</returns>
        public TutorAvailabilityView? GetAvailabilityById(int availabilityId)
        {
            return _tutorRepository.GetAvailabilityById(availabilityId);
        }

        /// <summary>
        /// Books an availability slot
        /// </summary>
        /// <param name="availabilityId">The availability slot ID</param>
        /// <returns>True if booking successful</returns>
        //public bool BookAvailability(int availabilityId)
        //{
        //    return _tutorRepository.BookAvailability(availabilityId);
        //}
        public PagedResult<TutorCard> GetTutors(TutorQuery query)
        {
            return _tutorRepository.GetTutors(query);
        }

       
    }
}
