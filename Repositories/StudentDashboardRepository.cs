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
                                SELECT * FROM booking AS b
                                INNER JOIN tutorAvailability AS ta ON b.tutorAvailabilityId = ta.tutorAvailabilityId
                                INNER JOIN users AS u ON u.personnelNumber = b.studentNumber
                                WHERE studentNumber = @StudentId";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    using (SqlDataReader read = cmd.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            AppointmentTable appointmentTable = new AppointmentTable();

                            appointmentTable.TutorName = read.GetString(18) + " " + read.GetString(19) ;
                            appointmentTable.Subject = read.GetString(11);
                            appointmentTable.AppointmentDate = read.GetDateTime(7);
                            appointmentTable.Status = read.GetString(8);

                            appointments.Add(appointmentTable);
                        }
                    }
                }
            }

            return appointments;
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
                                  AND dateBooked >= GETDATE()";

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


    }

    public class AppointmentTable {
        public string TutorName { get; set; } = "";
        public string Subject { get; set; } = "";
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = ""; 
    
    }
}
