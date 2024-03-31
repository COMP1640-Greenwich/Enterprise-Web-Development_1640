using _1640.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Areas.Repository;
using Microsoft.EntityFrameworkCore;

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
        public IEnumerable<Semester> GetAllOpening()
        {
            var semesters = _db.Semesters.AsEnumerable().Where(c => c.Status == "Opening").ToList();
            return semesters;
        }
    }
}
