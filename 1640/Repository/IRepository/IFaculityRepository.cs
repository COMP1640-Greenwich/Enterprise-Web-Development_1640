using _1640.Models;

namespace _1640.Repository.IRepository
{
    public interface IFaculityRepository: IRepository<Faculity>
    {
        public void Update(Faculity entity);
    }
}
