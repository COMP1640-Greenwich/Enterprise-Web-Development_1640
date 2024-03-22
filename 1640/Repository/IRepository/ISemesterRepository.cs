using _1640.Models;

namespace _1640.Areas.Repository.IRepository
{
    public interface ISemesterRepository : IRepository<Semester>
    {
        void Update(Semester entity);
    }
}
