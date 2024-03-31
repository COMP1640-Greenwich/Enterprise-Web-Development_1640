using _1640.Repository.IRepository;

namespace _1640.Areas.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISemesterRepository SemesterRepository { get; set; }
        IFacultyRepository FacultyRepository { get; set; }
        IArticleRepository ArticleRepository  { get; set; }
        IUserRepository UserRepository { get; set; }
        void Save();
    }
}
