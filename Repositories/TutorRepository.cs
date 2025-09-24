using System.Reflection.PortableExecutable;
using CampusLearn.Models;
using Microsoft.Data.SqlClient;

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

        // Paged/filtered tutors for All Tutors page
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
                                //ProfilePicture = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                ProfilePicture = reader.IsDBNull(1) ? "/Media/tutors/default.jpg": "/Media/tutors/" + reader.GetString(1),
                                AverageGrade = reader.GetDecimal(2),
                                FullName = $"{reader.GetString(3)} {reader.GetString(4)}",
                                YearsOfStudy = reader.GetInt32(5),
                                Program = reader.GetString(6)
                            };
                            result.Items.Add(tutor);
                        }
                    }
                }
               

                // load modules per tutor (optional for display badges)
                if (result.Items.Count > 0)
                {
                    string modulesSql = @"SELECT tm.tutorId, m.moduleCode
                                           FROM tutorModules tm
                                           JOIN modules m ON m.moduleCode = tm.moduleCode
                                           WHERE tm.tutorId IN (" + string.Join(",", result.Items.Select(t => t.TutorId)) + ")";

                    using (var modulesCmd = new SqlCommand(modulesSql, connectDB))
                    using (var modReader = modulesCmd.ExecuteReader())
                    {
                        var map = result.Items.ToDictionary(t => t.TutorId, t => t);
                        while (modReader.Read())
                        {
                            int tutorId = modReader.GetInt32(0);
                            string code = modReader.GetString(1);
                            if (map.TryGetValue(tutorId, out var t))
                            {
                                t.Modules.Add(code);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
