using CampusLearn.Repositories;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public class TutorService
    {
        private readonly TutorRepository _tutorRepository;
        public TutorService(TutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public List<TutorCard> GetTopTutors()
        {
            //get the top tutors from the repository
            var tutors = _tutorRepository.GetTopTutors();
            //ensure tutors have an average grade above 75 and at least 2 years of study
            tutors = tutors.Where(tutor => tutor.AverageGrade >= 75).ToList();
            return tutors;
        }
        public PagedResult<TutorCard> GetTutors(TutorQuery query)
        {
            return _tutorRepository.GetTutors(query);
        }

       
    }
}
