using _1640.Models;
using System.Linq.Expressions;

namespace _1640.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {
        void Update(Article entity);
        public IEnumerable<Article> GetAllApprove(string? includeProperty = null);
        IEnumerable<Article> GetAll(Expression<Func<Article, bool>> filter = null);

    }
}
