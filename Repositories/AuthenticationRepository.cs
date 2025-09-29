using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using CampusLearn.Models;

namespace CampusLearn.Repositories
{
    public class AuthenticationRepository
    {
        //get the connection string from the database 
        private readonly string _connectionString;

        public AuthenticationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        // Business rule: Check if personnel number already exists
        public bool PersonnelNumberExists(string personnelNumber)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                string query = @"SELECT COUNT(*) FROM users WHERE personnelNumber = @PersonnelNumber";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@PersonnelNumber", personnelNumber);
                    
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Business rule: Check if email already exists
        public bool EmailExists(string email)
        {
            using (SqlConnection connectDB = new SqlConnection(_connectionString))
            {
                connectDB.Open();

                // Use LOWER() to make email comparison case-insensitive and trim whitespace
                string query = @"SELECT COUNT(*) FROM users WHERE email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, connectDB))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        //method that will add student registrations
        public bool AddNewUser(string personnelNumber, string email, string password, string firstName, string lastName, string phoneNumber)
        {
            //connect to Database
            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();

                    //encrypt the password before adding to db
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string encrptedPassword = passwordHasher.HashPassword(new IdentityUser(), password);

                    //query to user to users table 
                    string addUserQuery = @"
                                            INSERT INTO users(personnelNumber,email,password,firstName,lastName,cellphone,role)
                                            VALUES(@PersonnelNumber, @Email, @EncryptedPassword, @FirstName,@LastName,@PhoneNumber, 'Student')";

                    //execute the query
                    using (SqlCommand cmd = new SqlCommand(addUserQuery, connectDB))
                    {
                        cmd.Parameters.AddWithValue("@PersonnelNumber", personnelNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@EncryptedPassword", encrptedPassword);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        // Return true if at least one row was affected (successful insert)
                        return rowsAffected > 0;
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the full exception details for debugging
                Console.WriteLine($"Error adding user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Re-throw the exception so the service layer knows it failed
                throw;
            }
        }

        public bool ValidateUser(string email, string password)
        {
            //connect to Database
            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();

                    //query to get user details from users table
                    string getUserQuery = @"SELECT password FROM users WHERE email = @Email";

                    using (SqlCommand cmd = new SqlCommand(getUserQuery, connectDB))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        var result = cmd.ExecuteScalar(); // Expecting a single value (the hashed password)
                        if (result != null)
                        {
                            string storedHashedPassword = result.ToString() ?? "";
                            var passwordHasher = new PasswordHasher<IdentityUser>();
                            var verificationResult = passwordHasher.VerifyHashedPassword(new IdentityUser(), storedHashedPassword, password);

                            // store the details using session data 
                            return verificationResult == PasswordVerificationResult.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details for debugging
                Console.WriteLine($"Error validating user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Re-throw the exception so the service layer knows it failed
                throw;

            }
            return false;
        }

        //method to get user details by email 
        public User GetUserByEmail(string email)
        {
            //User object to hold user details
            User user = new User();

            try
            {
                using (SqlConnection connectDB = new SqlConnection(_connectionString))
                {
                    connectDB.Open();

                    //query to get user details from users table
                    string getUserQuery = @"SELECT personnelNumber, email, firstName, role FROM users 
                                            WHERE email = @Email";

                    using (SqlCommand cmd = new SqlCommand(getUserQuery, connectDB))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user.PersonnelNumber = reader.GetString(0);
                                user.Email = reader.GetString(1);
                                user.FirstName = reader.GetString(2);
                                user.Role = reader.GetString(3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details for debugging
                Console.WriteLine($"Error retrieving user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Re-throw the exception so the service layer knows it failed
                throw;
            }


            return user;
        }
    }
}
