using _1640.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Areas.Repository;

namespace _1640.Repository
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
