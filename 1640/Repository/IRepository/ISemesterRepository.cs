using _1640.Models;
using System.Linq.Expressions;

namespace _1640.Repository.IRepository
{
    public interface ISemesterRepository : IRepository<Semester>
    {
        void Update(Semester entity);
        public IEnumerable<Semester> GetAllOpening();
        IEnumerable<Semester> GetAll(Expression<Func<Semester, bool>> filter = null);
    }
}
