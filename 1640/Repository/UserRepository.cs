using _1640.Data;
using _1640.Models;
using _1640.Repository.IRepository;

namespace _1640.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(User entity)
        {
            _db.Users.Update(entity);
        }
    }
}
