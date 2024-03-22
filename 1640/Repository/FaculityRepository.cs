using _1640.Repository.IRepository;
using _1640.Data;
using _1640.Models;

namespace _1640.Repository
{
    public class FaculityRepository : Repository<Faculity>, IFaculityRepository
    {
        private readonly ApplicationDbContext _db;
        public FaculityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Faculity entity)
        {
            _db.Faculities.Update(entity);
        }
    }
}
