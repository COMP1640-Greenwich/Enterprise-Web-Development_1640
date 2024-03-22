using _1640.Areas.Repository.IRepository;
using _1640.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace _1640.Areas.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> DbSet { get;  set; }
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            DbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentException(nameof(entity), " Entity cannot be null");
            }
            _db.Add(entity);
        }

        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperty = null) 
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            if (!String.IsNullOrEmpty(includeProperty))
            {
                query.Include(includeProperty).ToList();
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includeProperty = null)
        {
            IQueryable<T> query = DbSet;
            if (!String.IsNullOrEmpty(includeProperty))
            {
                query.Include(includeProperty).ToList();
            }
            return query.ToList();
        }
    }
}
