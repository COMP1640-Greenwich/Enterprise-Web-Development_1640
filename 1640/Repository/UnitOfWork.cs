using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Repository;
using _1640.Repository.IRepository;

namespace _1640.Areas.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IFacultyRepository FacultyRepository { get; set;}
        public ISemesterRepository SemesterRepository { get; set;}
        public IArticleRepository ArticleRepository { get; set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            FacultyRepository = new FacultyRepository(db);
            SemesterRepository = new SemesterRepository(db);
            ArticleRepository = new ArticleRepository(db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
