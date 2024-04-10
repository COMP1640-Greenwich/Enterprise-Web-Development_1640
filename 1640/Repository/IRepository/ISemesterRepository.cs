using _1640.Models;

namespace _1640.Repository.IRepository
{
    public interface ISemesterRepository : IRepository<Semester>
    {
        void Update(Semester entity);
        public IEnumerable<Semester> GetAllOpening();
        IEnumerable<Semester> GetAllOpeningByFaculty(Func<Semester, bool> predicate);

    }
}
