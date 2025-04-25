using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUser { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IProductImagesRepository ProductImages { get; }
        IContactUsRepository ContactUs { get; }



        void Commit();
    }
    
}
