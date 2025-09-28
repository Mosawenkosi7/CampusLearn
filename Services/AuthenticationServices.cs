using CampusLearn.Repositories;

namespace CampusLearn.Services
{
    public class AuthenticationServices
    {
        private readonly AuthenticationRepository _authRepository;
        public AuthenticationServices(AuthenticationRepository authRepository)
        {
            _authRepository = authRepository;
        }

       //method that will check if email or personnelNumber exist and then add user to database
       public bool AddNewUser(string personnelNumber, string email, string password, string firstName, string lastName, string phoneNumber)
        {
            try
            {
                //check if email exist in Db 
                if (_authRepository.EmailExists(email))
                {
                    return false;  //this should exit the AddNewUser method
                }

                //check if personnelNumber exist
                if (_authRepository.PersonnelNumberExists(personnelNumber))
                {
                    return false;
                }

                //if the two if's dont execute, then add user 
                bool success = _authRepository.AddNewUser(personnelNumber, email, password, firstName, lastName, phoneNumber);
                return success;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Authentication service error: {ex.Message}");
                return false;
            }
        }
    }

   
}
