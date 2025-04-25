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
            ApplicationUser = new ApplicationUserRepository(_dbContext);
            Category = new CategoryRepository(_dbContext);
            Product = new ProductRepository(_dbContext);
            ProductImages = new ProductImagesRepository(_dbContext);
            ContactUs = new ContactUsRepository(_dbContext);
        }
        public IApplicationUserRepository ApplicationUser { get; }
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public IProductImagesRepository ProductImages { get; }
        public IContactUsRepository ContactUs { get; }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
    
}
