using _1640.Models;

namespace _1640.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {
        void Update(Article entity);
        public IEnumerable<Article> GetAllApprove(string? includeProperty = null);
    }
}
