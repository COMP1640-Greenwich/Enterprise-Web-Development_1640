using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;

namespace _1640.Areas.Repository
{
    public class SemesterRepository : Repository<Semester>, ISemesterRepository
    {
        private readonly ApplicationDbContext _db;
        public SemesterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Semester entity)
        {
            _db.Semesters.Update(entity);
        }
    }
}
