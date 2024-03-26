using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Repository;

namespace _1640.Areas.Repository
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        private readonly ApplicationDbContext _db;
        public FacultyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Faculty entity)
        {
            _db.Faculties.Update(entity);
        }
    }
}
