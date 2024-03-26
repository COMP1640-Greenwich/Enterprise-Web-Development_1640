using _1640.Models;
using _1640.Repository.IRepository;

namespace _1640.Areas.Repository.IRepository
{
    public interface IFacultyRepository: IRepository<Faculty>
    {
        public void Update(Faculty entity);
    }
}
