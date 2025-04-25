using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ProductImagesRepository : Repository<ProductImages>, IProductImagesRepository
    {
        public ProductImagesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

}
