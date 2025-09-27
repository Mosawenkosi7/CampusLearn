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
            // Trim and normalize input data
            personnelNumber = personnelNumber?.Trim() ?? "";
            email = email?.Trim().ToLower() ?? "";
            firstName = firstName?.Trim() ?? "";
            lastName = lastName?.Trim() ?? "";
            phoneNumber = phoneNumber?.Trim() ?? "";

            Console.WriteLine($"AuthenticationServices: Validating user - Email: {email}, PersonnelNumber: {personnelNumber}");

            //check if email exist in Db 
            if (_authRepository.EmailExists(email))
            {
                Console.WriteLine($"AuthenticationServices: Email {email} already exists - validation failed");
                return false;  //this should exit the AddNewUser method
            }

            //check if personnelNumber exist
            if (_authRepository.PersonnelNumberExists(personnelNumber))
            {
                Console.WriteLine($"AuthenticationServices: PersonnelNumber {personnelNumber} already exists - validation failed");
                return false;
            }

            Console.WriteLine($"AuthenticationServices: Validation passed - proceeding to add user to database");
            //if the two if's dont execute, then add user 
            bool result = _authRepository.AddNewUser(personnelNumber, email, password, firstName, lastName, phoneNumber);
            Console.WriteLine($"AuthenticationServices: Database insertion result: {result}");
            return result;
        }
    }

   
}
