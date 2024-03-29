using _1640.Areas.Repository;
using _1640.Data;
using _1640.Models;
using _1640.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace _1640.Repository
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ArticleRepository(ApplicationDbContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public void Update(Article entity)
        {
            _dbContext.Articles.Update(entity);
        }
        public IEnumerable<Article> GetAllApprove(string? includeProperty = null)
        {
            var articles = _dbContext.Articles.Where(c => c.Status == Article.StatusArticle.Approve).ToList();
            return articles;
        }

    }
}
