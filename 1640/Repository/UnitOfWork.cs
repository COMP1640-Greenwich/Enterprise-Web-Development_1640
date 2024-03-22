using _1640.Areas.Repository.IRepository;
using _1640.Data;

namespace _1640.Areas.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IFaculityRepository FaculityRepository { get; set;}
        public ISemesterRepository SemesterRepository { get; set;}
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            FaculityRepository = new FaculityRepository(db);
            SemesterRepository = new SemesterRepository(db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
