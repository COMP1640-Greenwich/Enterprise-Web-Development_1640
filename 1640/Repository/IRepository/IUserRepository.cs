using _1640.Models;

namespace _1640.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        void Update(User entity);
    }
}
