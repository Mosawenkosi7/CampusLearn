using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;

namespace CampusLearn.Repositories
{
    public class TutorRepository
    {
        //coonect to DB

        private readonly string _connectionString;  
        public TutorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        //method that returns a list of top 3 tutors
        public List<TutorCard> GetTopTutors()
        {
            var tutors = new List<TutorCard>(); //list of tutors

            //connect to DB
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open(); //open connection

                //query to get top 3 tutors based on average grade and years of study
                string query = @"
                    SELECT TOP 3 tp.tutorId,tp.profilePicture,tp.academicAverage, users.firstName, users.lastName, sp.yearOfStudy, sp.program
                    FROM tutorProfile AS tp
                    INNER JOIN users ON tp.studentNumber = users.personnelNumber
                    INNER JOIN studentProfile AS sp ON sp.studentNumber = tp.StudentNumber
                    ORDER BY tp.academicAverage DESC;
                    ";

                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    //execute query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tutor = new TutorCard
                            {
                                TutorId = reader.GetInt32(0),
                                ProfilePicture = reader.GetString(1),
                                AverageGrade = reader.GetDecimal(2),
                                FullName = $"{reader.GetString(3)} {reader.GetString(4)}",
                                YearsOfStudy = reader.GetInt32(5),
                                Program = reader.GetString(6)
                            };
                            tutors.Add(tutor);
                        }
                    }
                }
            }

            return tutors;
        }

        //method to get tutor profile by ID
        public TutorCard? GetTutorProfile(int tutorId)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"
                    SELECT tp.tutorId, tp.profilePicture, tp.academicAverage, tp.tutorSummary, 
                           users.firstName, users.lastName, sp.yearOfStudy, sp.program
                    FROM tutorProfile AS tp
                    INNER JOIN studentProfile AS sp ON tp.studentNumber = sp.studentNumber
                    INNER JOIN users ON sp.studentNumber = users.personnelNumber
                    WHERE tp.tutorId = @tutorId";

                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    command.Parameters.AddWithValue("@tutorId", tutorId);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TutorCard
                            {
                                TutorId = reader.GetInt32(0),
                                ProfilePicture = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                AverageGrade = reader.GetDecimal(2),
                                Bio = reader.IsDBNull(3) ? "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book." : reader.GetString(3),
                                FullName = $"{reader.GetString(4)} {reader.GetString(5)}",
                                YearsOfStudy = reader.GetInt32(6),
                                Program = reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        //method to get tutor availability
        public List<TutorAvailability> GetTutorAvailability(int tutorId)
        {
            var availabilities = new List<TutorAvailability>();

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"
                    SELECT ta.tutorAvailabilityId, m.moduleName, ta.available,
                           CASE WHEN b.bookingId IS NOT NULL THEN 1 ELSE 0 END as isBooked
                    FROM tutorAvailability ta
                    INNER JOIN modules m ON ta.moduleCode = m.moduleCode
                    LEFT JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId
                    WHERE ta.tutorId = @tutorId AND ta.available >= GETDATE()
                    ORDER BY ta.available";

                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    command.Parameters.AddWithValue("@tutorId", tutorId);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var availableDateTime = reader.GetDateTime(2);
                            availabilities.Add(new TutorAvailability
                            {
                                AvailabilityId = reader.GetInt32(0),
                                Module = reader.GetString(1),
                                AvailableDate = availableDateTime.Date,
                                StartTime = availableDateTime.TimeOfDay,
                                EndTime = availableDateTime.AddHours(1).TimeOfDay, // Assuming 1-hour sessions
                                IsBooked = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            return availabilities;
        }

        //method to book availability
        public bool BookAvailability(int availabilityId, string studentNumber = "ST12345678")
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                // Check if availability exists and is not already booked
                string checkQuery = @"
                    SELECT COUNT(*) FROM tutorAvailability ta
                    LEFT JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId
                    WHERE ta.tutorAvailabilityId = @availabilityId AND b.bookingId IS NULL";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connectDB))
                {
                    checkCommand.Parameters.AddWithValue("@availabilityId", availabilityId);
                    int availableCount = (int)checkCommand.ExecuteScalar();
                    
                    if (availableCount == 0)
                        return false;
                }

                // Create booking record
                string insertQuery = @"
                    INSERT INTO booking (studentNumber, tutorAvailabilityId, location, bookingSummary, dateBooked, status)
                    VALUES (@studentNumber, @availabilityId, 'Online Session', 'Tutoring session booked through CampusLearn', GETDATE(), 'Active')";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connectDB))
                {
                    insertCommand.Parameters.AddWithValue("@studentNumber", studentNumber);
                    insertCommand.Parameters.AddWithValue("@availabilityId", availabilityId);
                    return insertCommand.ExecuteNonQuery() > 0;
                }
            }
        }
    }

    public class TutorCard
    {
        public int TutorId { get; set; }
        public decimal AverageGrade { get; set; }
        public int YearsOfStudy { get; set; } 
        public string FullName { get; set; } = "";
        public string Program { get; set; } = "";
        public string ProfilePicture { get; set; } = "";

        public string Bio { get; set; } = "";
    }



    public class TutorAvailability
    {
        public int AvailabilityId { get; set; }
        public string Module { get; set; } = "";
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBooked { get; set; }
    }
}
