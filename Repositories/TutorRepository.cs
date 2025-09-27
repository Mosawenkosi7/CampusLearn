using System.Reflection.PortableExecutable;
using CampusLearn.Models;
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

        /// <summary>
        /// Gets the top 3 tutors based on academic average
        /// </summary>
        /// <returns>List of top tutors</returns>
        public List<TutorCard> GetTopTutors()
        {
            var tutors = new List<TutorCard>();

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                // Query to get top 3 tutors based on academic average
                string query = @"
                    SELECT TOP 3 tp.tutorId, tp.profilePicture, tp.academicAverage, tp.tutorSummary,
                           users.firstName, users.lastName, sp.yearOfStudy, sp.program, users.email
                    FROM tutorProfile AS tp
                    INNER JOIN users ON tp.studentNumber = users.personnelNumber
                    INNER JOIN studentProfile AS sp ON sp.studentNumber = tp.StudentNumber
                    ORDER BY tp.academicAverage DESC;
                    ";

                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tutor = new TutorCard
                            {
                                TutorId = reader.GetInt32(0),
                                ProfilePicture = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                AcademicAverage = reader.GetDecimal(2),
                                TutorSummary = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                FirstName = reader.GetString(4),
                                LastName = reader.GetString(5),
                                YearOfStudy = reader.GetInt32(6),
                                Program = reader.GetString(7),
                                Campus = "", // Default empty since campus column doesn't exist
                                Email = reader.GetString(8)
                            };
                            tutors.Add(tutor);
                        }
                    }
                }
            }

            return tutors;
        }

        /// <summary>
        /// Gets a specific tutor's profile by ID
        /// </summary>
        /// <param name="tutorId">The tutor's ID</param>
        /// <returns>TutorCard or null if not found</returns>
        public TutorCard? GetTutorProfile(int tutorId)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"
                    SELECT tp.tutorId, tp.profilePicture, tp.academicAverage, tp.tutorSummary, 
                           users.firstName, users.lastName, sp.yearOfStudy, sp.program, users.email
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
                                AcademicAverage = reader.GetDecimal(2),
                                TutorSummary = reader.IsDBNull(3) ? "No bio available yet." : reader.GetString(3),
                                FirstName = reader.GetString(4),
                                LastName = reader.GetString(5),
                                YearOfStudy = reader.GetInt32(6),
                                Program = reader.GetString(7),
                                Campus = "", // Default empty since campus column doesn't exist
                                Email = reader.GetString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets available time slots for a tutor, excluding overlapping sessions
        /// </summary>
        /// <param name="tutorId">The tutor's ID</param>
        /// <returns>List of available time slots</returns>
        public List<TutorAvailabilityView> GetTutorAvailability(int tutorId)
        {
            var availabilities = new List<TutorAvailabilityView>();

            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();

                    string query = @"
                        WITH BookedSessions AS (
                            -- Get all booked sessions with their time ranges (assuming 1-hour sessions)
                            SELECT ta.tutorId, ta.available as startTime, 
                                   DATEADD(HOUR, 1, ta.available) as endTime
                            FROM tutorAvailability ta
                            INNER JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId
                            WHERE ta.tutorId = @tutorId AND b.status = 'Active'
                        )
                        SELECT ta.tutorAvailabilityId, ta.tutorId, ta.moduleCode, ta.available,
                               CASE WHEN b.bookingId IS NOT NULL THEN 1 ELSE 0 END as isBooked
                        FROM tutorAvailability ta
                        LEFT JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId
                        WHERE ta.tutorId = @tutorId 
                          AND ta.available >= GETDATE()
                          -- Exclude slots that would overlap with existing booked sessions
                          AND NOT EXISTS (
                              SELECT 1 FROM BookedSessions bs
                              WHERE (
                                  -- New session would start during an existing session
                                  (ta.available >= bs.startTime AND ta.available < bs.endTime)
                                  OR
                                  -- New session would end during an existing session  
                                  (DATEADD(HOUR, 1, ta.available) > bs.startTime AND DATEADD(HOUR, 1, ta.available) <= bs.endTime)
                                  OR
                                  -- New session would completely contain an existing session
                                  (ta.available <= bs.startTime AND DATEADD(HOUR, 1, ta.available) >= bs.endTime)
                              )
                          )
                        ORDER BY ta.available ASC";

                    using (SqlCommand command = new SqlCommand(query, connectDB))
                    {
                        command.Parameters.AddWithValue("@tutorId", tutorId);
                        
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var availableDateTime = reader.GetDateTime(3);
                                var availability = new TutorAvailabilityView
                                {
                                    TutorAvailabilityId = reader.GetInt32(0),
                                    TutorId = reader.GetInt32(1),
                                    ModuleCode = reader.GetString(2),
                                    Available = availableDateTime,
                                    Location = "Online", // Default since location column doesn't exist
                                    IsActive = true, // Default since isActive column doesn't exist
                                    IsBooked = reader.GetInt32(4) == 1
                                };
                                availabilities.Add(availability);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging but don't crash the application
                Console.WriteLine($"Error getting tutor availability: {ex.Message}");
            }

            return availabilities;
        }



        /// <summary>
        /// Gets a specific availability by ID with tutor and module details
        /// </summary>
        /// <param name="availabilityId">The availability ID</param>
        /// <returns>TutorAvailabilityView or null if not found</returns>
        public TutorAvailabilityView? GetAvailabilityById(int availabilityId)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"
                    SELECT ta.tutorAvailabilityId, ta.tutorId, ta.moduleCode, ta.available,
                           m.moduleName,
                           tp.academicAverage, tp.tutorSummary,
                           u.firstName, u.lastName, u.email,
                           CASE WHEN b.bookingId IS NOT NULL THEN 1 ELSE 0 END as isBooked
                    FROM tutorAvailability ta
                    INNER JOIN modules m ON ta.moduleCode = m.moduleCode
                    INNER JOIN tutorProfile tp ON ta.tutorId = tp.tutorId
                    INNER JOIN studentProfile sp ON tp.studentNumber = sp.studentNumber
                    INNER JOIN users u ON sp.studentNumber = u.personnelNumber
                    LEFT JOIN booking b ON ta.tutorAvailabilityId = b.tutorAvailabilityId AND b.status = 'Active'
                    WHERE ta.tutorAvailabilityId = @availabilityId";

                using (SqlCommand command = new SqlCommand(query, connectDB))
                {
                    command.Parameters.AddWithValue("@availabilityId", availabilityId);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var availableDateTime = reader.GetDateTime(3);
                            return new TutorAvailabilityView
                            {
                                TutorAvailabilityId = reader.GetInt32(0),
                                TutorId = reader.GetInt32(1),
                                ModuleCode = reader.GetString(2),
                                Available = availableDateTime,
                                Location = "Online", // Default since location column doesn't exist in tutorAvailability
                                IsActive = true, // Default since isActive column doesn't exist
                                IsBooked = reader.GetInt32(10) == 1
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Books an availability slot for a student
        /// </summary>
        /// <param name="availabilityId">The availability slot ID</param>
        /// <param name="studentNumber">The student's number (default for demo)</param>
        /// <returns>True if booking successful, false otherwise</returns>
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

        /// <summary>
        /// Gets paged and filtered tutors for All Tutors page
        /// </summary>
        /// <param name="query">Query parameters for filtering and paging</param>
        /// <returns>Paged result of tutors</returns>
        public PagedResult<TutorCard> GetTutors(TutorQuery query)
        {
            var result = new PagedResult<TutorCard>
            {
                Page = query.Page,
                PageSize = query.PageSize
            };

            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                // count query with filters
                string countSql = @"
                    SELECT COUNT(*)
                    FROM tutorProfile tp
                    INNER JOIN users u ON tp.studentNumber = u.personnelNumber
                    INNER JOIN studentProfile sp ON sp.studentNumber = tp.studentNumber
                    WHERE ( @search IS NULL OR (u.firstName + ' ' + u.lastName LIKE '%' + @search + '%') )
                      AND ( @moduleCode IS NULL OR EXISTS (
                            SELECT 1 FROM tutorModules tm WHERE tm.tutorId = tp.tutorId AND tm.moduleCode = @moduleCode
                          ) )
                      AND ( @yearOfStudy IS NULL OR sp.yearOfStudy = @yearOfStudy );";

                using (var countCmd = new SqlCommand(countSql, connectDB))
                {
                    countCmd.Parameters.AddWithValue("@search", (object?)query.Search ?? DBNull.Value);
                    countCmd.Parameters.AddWithValue("@moduleCode", (object?)query.ModuleCode ?? DBNull.Value);
                    countCmd.Parameters.AddWithValue("@yearOfStudy", (object?)query.YearOfStudy ?? DBNull.Value);
                    result.TotalCount = (int)countCmd.ExecuteScalar();
                }

                // main query with pagination
                string dataSql = @"
                    WITH filtered AS (
                        SELECT tp.tutorId,
                               tp.profilePicture,
                               tp.academicAverage,
                               u.firstName,
                               u.lastName,
                               sp.yearOfStudy,
                               sp.program
                        FROM tutorProfile tp
                        INNER JOIN users u ON tp.studentNumber = u.personnelNumber
                        INNER JOIN studentProfile sp ON sp.studentNumber = tp.studentNumber
                        WHERE ( @search IS NULL OR (u.firstName + ' ' + u.lastName LIKE '%' + @search + '%') )
                          AND ( @moduleCode IS NULL OR EXISTS (
                                SELECT 1 FROM tutorModules tm WHERE tm.tutorId = tp.tutorId AND tm.moduleCode = @moduleCode
                              ) )
                          AND ( @yearOfStudy IS NULL OR sp.yearOfStudy = @yearOfStudy )
                    )
                    SELECT *
                    FROM filtered
                    ORDER BY academicAverage DESC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

                using (var cmd = new SqlCommand(dataSql, connectDB))
                {
                    int offset = (query.Page - 1) * query.PageSize;
                    cmd.Parameters.AddWithValue("@search", (object?)query.Search ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@moduleCode", (object?)query.ModuleCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@yearOfStudy", (object?)query.YearOfStudy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@pageSize", query.PageSize);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tutor = new TutorCard
                            {
                                TutorId = reader.GetInt32(0),
                                ProfilePicture = reader.IsDBNull(1) ? "/Media/tutors/default.jpg": "/Media/tutors/" + reader.GetString(1),
                                AcademicAverage = reader.GetDecimal(2),
                                FirstName = reader.GetString(3),
                                LastName = reader.GetString(4),
                                YearOfStudy = reader.GetInt32(5),
                                Program = reader.GetString(6),
                                Campus = "",
                                Email = ""
                            };
                            result.Items.Add(tutor);
                        }
                    }
                }
               

                // Note: Modules functionality removed to maintain clean TutorCard structure
            }

            return result;
        }
    }
}
