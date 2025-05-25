using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

using System.Threading.Tasks;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet("")]
        public IActionResult GetAll([FromQuery] int PageNum = 1, [FromQuery] int pagesize = 30)
        {
            var products = _unitOfWork.ProductRepository.Get().Skip((PageNum - 1) * pagesize).Take(pagesize).ToList();
            return Ok(products.Adapt<List<ProductResDTO>>());
        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOneProduct([FromRoute] int id)
        {
            var prod = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
            if (prod == null)
                return NotFound();
            return Ok(prod.Adapt<ProductResDTO>());
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("")]
        public async Task<IActionResult> CreateProductAsync([FromForm] ProductReqDTO prod , CancellationToken cancellationToken)
        {
            List<string> newFiles = new List<string>();
            if (prod.Files!=null &&  prod.Files.Any())
            {
                foreach (var file in prod.Files)
                {
                    // to cancel the request if CancelationToken was true
                    cancellationToken.ThrowIfCancellationRequested();
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream , cancellationToken);
                        }
                        newFiles.Add(fileName);
                    }
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            Product product = prod.Adapt<Product>();
            var CreatedProd =await _unitOfWork.ProductRepository.CreateAsync(product , cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            if (newFiles.Count > 0)
            {
                foreach (var file in newFiles)
                {
                    await _unitOfWork.ProductImagesRepository.CreateAsync(new ProductImages()
                    {
                        ImageUrl = file,
                        ProductId = product.Id
                    } , cancellationToken);
                }
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            return Created($"{Request.Scheme}://{Request.Host}/api/Products/{product.Id}", CreatedProd.Adapt<ProductResDTO>());
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> EditProductAsync([FromRoute] int id, [FromForm] ProductReqDTO prod , CancellationToken cancellationToken)
        {
            var productDB = _unitOfWork.ProductRepository.GetOne(e => e.Id == id, tracked: false);
            if (productDB == null) return NotFound();
            prod.Id = productDB.Id;
            List<string> newFiles = new List<string>();
            if ( prod.Files != null &&prod.Files.Any())
            {
                List<ProductImages> prodImgDB = _unitOfWork.ProductImagesRepository.Get(e => e.ProductId == id).ToList();

                foreach (var file in prod.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                        cancellationToken.ThrowIfCancellationRequested();
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream , cancellationToken);
                        }
                        newFiles.Add(fileName);
                    }

                }
                foreach (var img in prodImgDB)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "images", img.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                        _unitOfWork.ProductImagesRepository.Delete(img);
                    }
                }
                foreach (var file in newFiles)
                {
                    await _unitOfWork.ProductImagesRepository.CreateAsync(new ProductImages()
                    {
                        ImageUrl = file,
                        ProductId = productDB.Id
                    }, cancellationToken);
                }
                await _unitOfWork.CommitAsync(cancellationToken);

            }

            Product product = prod.Adapt<Product>();
            _unitOfWork.ProductRepository.Alter(product);
            await _unitOfWork.CommitAsync(cancellationToken);
            return NoContent();

        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync( [FromRoute] int id, CancellationToken cancellationToken)
        {
            var prod = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
            if (prod == null) return NotFound();
            var imgs = _unitOfWork.ProductImagesRepository.Get(e => e.ProductId == id).ToList();
            _unitOfWork.ProductRepository.Delete(prod);
            if (imgs.Count > 0)
            {
                foreach (var img in imgs)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", img.ImageUrl);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    _unitOfWork.ProductImagesRepository.Delete(img);
                }
            }
            await _unitOfWork.CommitAsync(cancellationToken);
            return NoContent();
        }

    }
}
