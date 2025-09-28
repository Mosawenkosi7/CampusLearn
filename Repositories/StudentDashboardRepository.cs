using CampusLearn.Models;
using Microsoft.Data.SqlClient;

namespace CampusLearn.Repositories
{
    public class StudentDashboardRepository
    {
        //get the connection string
        private readonly string _connectionString;

        public StudentDashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        //write method to fetch data for all the booking for student 
        public List<AppointmentTable> GetBookings(string studentId)
        {
            var appointments = new List<AppointmentTable>();

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"
                                SELECT b.bookingId,u.firstName,u.lastName,ta.moduleCode,ta.available, b.status FROM booking AS b
                                INNER JOIN tutorAvailability AS ta ON b.tutorAvailabilityId = ta.tutorAvailabilityId
                                INNER JOIN tutorProfile AS tp ON ta.tutorId = tp.tutorId
                                INNER JOIN users AS u ON u.personnelNumber = tp.studentNumber
                                WHERE b.studentNumber = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    using (SqlDataReader read = cmd.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            AppointmentTable appointmentTable = new AppointmentTable();
                            appointmentTable.Booking.BookingId = read.GetInt32(0);
                            appointmentTable.User.FirstName = read.GetString(1);
                            appointmentTable.User.LastName = read.GetString(2);
                            appointmentTable.TutorAvailability.ModuleCode = read.GetString(3);
                            appointmentTable.TutorAvailability.Available = read.GetDateTime(4); 
                            appointmentTable.Booking.Status = read.GetString(5);
                            appointments.Add(appointmentTable);
                        }
                    }
                }
            }

            return appointments;
        }


        //method that will cancel a booking
        public void CancelBooking(int bookingId)
        {
            //conncect to database
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                //open the connection
                connectDB.Open();

                //query to delete the booking
                string deleteQuery = @"
                                        DELETE FROM booking
                                        WHERE bookingId = @BookingId";

                //execute the query 
                using (SqlCommand cmd = new SqlCommand(deleteQuery, connectDB))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // method that calculates number of sessions booked per student
        public int GetBookedSessionsCount(string studentId)
        {
            int totalBookedSessions = 0;

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"SELECT COUNT(*)
                                  FROM booking
                                  WHERE studentNumber = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalBookedSessions = Convert.ToInt32(result);
                    }
                }
            }

            return totalBookedSessions;
        }

        // method that calculates number of upcoming sessions
        public int GetUpComingSessions(string studentId)
        {
            int totalBookedSessions = 0;

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"SELECT COUNT(*)
                                  FROM booking
                                  WHERE studentNumber = @StudentId
                                  AND status = 'Active'";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalBookedSessions = Convert.ToInt32(result);
                    }
                }
            }

            return totalBookedSessions;
        }


        // method that calculates number of upcoming sessions
        public int GetCompletedSessions(string studentId)
        {
            int totalBookedSessions = 0;

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"SELECT COUNT(*)
                                  FROM booking
                                  WHERE studentNumber = @StudentId
                                  AND status = 'completed'";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalBookedSessions = Convert.ToInt32(result);
                    }
                }
            }

            return totalBookedSessions;
        }

        public int GetPendingSessions(string studentId)
        {
            int totalBookedSessions = 0;

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"SELECT COUNT(*)
                                  FROM booking
                                  WHERE studentNumber = @StudentId
                                  AND status = 'Pending'";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalBookedSessions = Convert.ToInt32(result);
                    }
                }
            }

            return totalBookedSessions;
        }

        public List<LearningResourceCard> GetLearningResources(string studentId)
        {
            List<LearningResourceCard> learningResourceCards = new List<LearningResourceCard>();
            //CONNECT TO db
            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();

                    //query to fetch resource card
                    string query = @"
                                    SELECT lr.resourceID,s.studentNumber,s.moduleCode,lr.topic,lr.description FROM subscription AS s 
                                    INNER JOIN learningResources AS lr ON s.moduleCode = lr.moduleCode
                                    WHERE s.studentNumber = @StudentId";

                    //execute the query 
                    using (SqlCommand cmd = new SqlCommand(query,connectDB))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        //read each row and store in the list
                        using (SqlDataReader read = cmd.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                LearningResourceCard learningResourceCard = new LearningResourceCard();
                                learningResourceCard.LearningResource.ResourceId = read.GetInt32(0);
                                learningResourceCard.Subscription.StudentName = read.GetString(1);
                                learningResourceCard.Subscription.ModuleCode = read.GetString(2);
                                learningResourceCard.LearningResource.Topic = read.GetString(3);
                                learningResourceCard.LearningResource.Description = read.GetString(4);

                                learningResourceCards.Add(learningResourceCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return learningResourceCards;
        }
    }

    public class AppointmentTable {
        public Booking Booking { get; set; } = new Booking(); //i need date booked, status, bookingId

        public User User { get; set; } = new User(); //get the tutor name

        public TutorAvailability TutorAvailability { get; set; } = new TutorAvailability(); //we want the module code
    }

    public class LearningResourceCard
    {
        public Subscription Subscription { get; set; } = new Subscription(); //store the student Number, ModuleCode
        public LearningResource LearningResource { get; set; } = new LearningResource(); //store topic,description,resourceId
    }
}
