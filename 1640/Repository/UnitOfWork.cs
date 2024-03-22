using _1640.Repository.IRepository;
using _1640.Data;
using Microsoft.EntityFrameworkCore;

namespace _1640.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IFaculityRepository FaculityRepository { get; set;}
        public ISemesterRepository SemesterRepository { get; set;}
        public IArticleRepository ArticleRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            FaculityRepository = new FaculityRepository(db);
            SemesterRepository = new SemesterRepository(db);
            ArticleRepository = new ArticleRepository(db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
