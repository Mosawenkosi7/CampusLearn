using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn.Models
{

    // Updated User model to match the new registration fields.
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AcademicProgram { get; set; }
        public string YearOfStudy { get; set; }
        public string Password { get; set; } // NOTE: Hash this in a real app!
    }

// This is the database context class that represents the database session.
// It inherits from DbContext, which is provided by Entity Framework Core.
public class UserDbContext : DbContext
    {
        // The constructor takes the database options and passes them to the base class.
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        // This DbSet property represents the 'Users' table in your database.
        // The User class is your C# model that will be mapped to a table.
        public DbSet<User> Users { get; set; }
    }

}

