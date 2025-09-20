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
    }

    public class TutorCard
    {
        public int TutorId { get; set; }
        public decimal AverageGrade { get; set; }
        public int YearsOfStudy { get; set; } 
        public string FullName { get; set; } = "";
        public string Program { get; set; } = "";

        public string ProfilePicture { get; set; } = "";
    }
}
