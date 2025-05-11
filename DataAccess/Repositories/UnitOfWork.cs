using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            ApplicationUserRepository = new ApplicationUserRepository(_dbContext);
            BrandRepository = new BrandRepository(_dbContext);
            ProductRepository = new ProductRepository(_dbContext);
            ProductImagesRepository = new ProductImagesRepository(_dbContext);
            ContactUsRepository = new ContactUsRepository(_dbContext);
            CartRepository = new CartRepository(_dbContext);
        }
        public IApplicationUserRepository ApplicationUserRepository { get; }
        public IBrandRepository BrandRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductImagesRepository ProductImagesRepository { get; }
        public IContactUsRepository ContactUsRepository { get; }
        public ICartRepository CartRepository { get; }
        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
        }

        public void Dispose()
        {
             _dbContext.Dispose();
        }

        
    }
    
}
