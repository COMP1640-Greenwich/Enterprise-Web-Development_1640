namespace _1640.Areas.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISemesterRepository SemesterRepository { get; set; }
        IFaculityRepository FaculityRepository { get; set;}
        void Save();
    }
}
