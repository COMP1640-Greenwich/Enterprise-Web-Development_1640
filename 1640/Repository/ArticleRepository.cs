using _1640.Data;
using _1640.Models;
using _1640.Repository.IRepository;

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
    }
}
