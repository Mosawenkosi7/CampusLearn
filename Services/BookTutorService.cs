using CampusLearn.Models;
using CampusLearn.Repositories;

namespace CampusLearn.Services
{
    public class BookTutorService
    {
        private readonly BookTutorRepository _bookTutorRepository;
        
        public BookTutorService(BookTutorRepository bookTutorRepository)
        {
            _bookTutorRepository = bookTutorRepository;
        }
        
        /// <summary>
        /// Creates a new booking for a student
        /// </summary>
        /// <param name="availabilityId">The availability slot ID</param>
        /// <param name="studentNumber">The student's number</param>
        /// <param name="location">The location for the session</param>
        /// <param name="bookingSummary">The booking summary</param>
        /// <param name="document1">First uploaded document</param>
        /// <param name="document2">Second uploaded document</param>
        /// <returns>True if booking successful</returns>
        public bool CreateBooking(int availabilityId, string studentNumber, string location, 
                                 string bookingSummary, string? document1 = null, string? document2 = null)
        {
            return _bookTutorRepository.CreateBooking(availabilityId, studentNumber, location, 
                                                    bookingSummary, document1, document2);
        }
        
        /// <summary>
        /// Gets student information by personnel number
        /// </summary>
        /// <param name="personnelNumber">The student's personnel number</param>
        /// <returns>User object or null if not found</returns>
        public User? GetStudentByPersonnelNumber(string personnelNumber)
        {
            return _bookTutorRepository.GetStudentByPersonnelNumber(personnelNumber);
        }
    }
}
