namespace _1640.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISemesterRepository SemesterRepository { get; set; }
        IFaculityRepository FaculityRepository { get; set;}
        IArticleRepository ArticleRepository { get; }
        void Save();
    }
}
