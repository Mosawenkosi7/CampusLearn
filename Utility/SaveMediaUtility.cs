using System.Threading.Tasks;

namespace CampusLearn.Utility
{
    public class SaveMediaUtility
    {
        //create dependency injection
        private readonly IWebHostEnvironment _environment; //gives us access to wwwroot folder 

        public SaveMediaUtility(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        //create a method that will store media into correct folders
        public async Task<string?> SaveAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            // Build the full folder path
            string folder = Path.Combine(_environment.WebRootPath, "Media", folderName);
            Directory.CreateDirectory(folder); // Ensure folder exists

            // Generate unique timestamp-based filename
            string fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName);

            // Full path to save the file
            string filePath = Path.Combine(folder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the saved file name (you can store this in the DB)
            return fileName;
        }
    }
}
