using Microsoft.Data.SqlClient;
using CampusLearn.Models;

namespace CampusLearn.Repositories
{
    public class BookTutorRepository
    {
        private readonly string _connectionString;
        
        public BookTutorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }
        
        /// <summary>
        /// Creates a new booking in the database
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
            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();
                    
                    // Check if availability exists and is not already booked
                    string checkQuery = @"
                        SELECT COUNT(*) FROM tutorAvailability ta
                        LEFT JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId AND b.status = 'Active'
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
                        INSERT INTO booking (studentNumber, tutorAvailabilityId, location, bookingSummary, document1, document2, dateBooked, status)
                        VALUES (@studentNumber, @availabilityId, @location, @bookingSummary, @document1, @document2, GETDATE(), 'Active')";
                    
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connectDB))
                    {
                        insertCommand.Parameters.AddWithValue("@studentNumber", studentNumber);
                        insertCommand.Parameters.AddWithValue("@availabilityId", availabilityId);
                        insertCommand.Parameters.AddWithValue("@location", location);
                        insertCommand.Parameters.AddWithValue("@bookingSummary", bookingSummary);
                        insertCommand.Parameters.AddWithValue("@document1", document1 ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@document2", document2 ?? (object)DBNull.Value);
                        
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        
                        bool success = rowsAffected > 0;
                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository exception: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets student information by personnel number
        /// </summary>
        /// <param name="personnelNumber">The student's personnel number</param>
        /// <returns>User object or null if not found</returns>
        public User? GetStudentByPersonnelNumber(string personnelNumber)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();
                
                string query = @"
                    SELECT personnelNumber, firstName, lastName, email, password, role, date_created, last_login
                    FROM users 
                    WHERE personnelNumber = @personnelNumber AND role = 'Student'";
                
                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    command.Parameters.AddWithValue("@personnelNumber", personnelNumber);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                PersonnelNumber = reader.GetString(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                Password = reader.GetString(4),
                                Role = reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6),
                                LastLogin = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
