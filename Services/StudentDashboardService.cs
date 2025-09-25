using CampusLearn.Repositories;

namespace CampusLearn.Services
{
    public class StudentDashboardService
    {
        private readonly StudentDashboardRepository _studentDashboardRepository;

        public StudentDashboardService(StudentDashboardRepository studentDashboardRepository)
        {
            _studentDashboardRepository = studentDashboardRepository;
        }

        //method to get the bookings for a student and apply business rules 
        public List<AppointmentTable> GetAppointmentTables(string studentId)
        {
            // get the data from Repository 
            var appointmentTables = _studentDashboardRepository.GetBookings(studentId);

            var now = DateTime.Now;
            var upcoming = appointmentTables
                .Where(a => a.TutorAvailability.Available >= now)
                .OrderBy(a => a.TutorAvailability.Available);

            var past = appointmentTables
                .Where(a => a.TutorAvailability.Available < now)
                .OrderByDescending(a => a.TutorAvailability.Available);

            return upcoming.Concat(past).ToList();
        }

        public List<LearningResourceCard> GetLearningResources(string studentId)
        {
            return _studentDashboardRepository.GetLearningResources(studentId);
        }


        //method that deletes row in database
        public bool CancelQuery(int bookingId, string status) //returns true or false(
        {
            if (status == "Active" || status == "Pending")
            {
                _studentDashboardRepository.CancelBooking(bookingId);
                return true;
            }
            return false;
        }

        //create method that will count number of sessions booked
        public int BookedSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetBookedSessionsCount(studentId);
            return totalSessionsCount;
        }

        //create method that will count number of sessions booked
        public int UpcomingSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetUpComingSessions(studentId);
            return totalSessionsCount;
        }

        //create method that will count completed sessions
        public int CompletedSessions(string studentId)
        {
            int totalSessionsCount = 0;

            totalSessionsCount = _studentDashboardRepository.GetCompletedSessions(studentId);
            return totalSessionsCount;
        }

        //method for counting pending sessions
        public int PendingSessionsCount(string studentId)
        {
            int totalSessionsCount = 0;
            totalSessionsCount = _studentDashboardRepository.GetPendingSessions(studentId);

            return totalSessionsCount;
        }

    }
}
