using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Models.DTOs;
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

        [HttpGet("")]
        public IActionResult GetAll([FromQuery] int PageNum = 1, [FromQuery] int pagesize = 30)
        {
            var products = _unitOfWork.ProductRepository.Get().Skip((PageNum - 1) * pagesize).Take(pagesize).ToList();
            return Ok(products.Adapt<List<ProductReqDTO>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOneProduct([FromRoute] int id)
        {
            var prod = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
            if (prod == null)
                return NotFound();
            return Ok(prod.Adapt<ProductReqDTO>());
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductReqDTO prod)
        {
            List<string> newFiles = new List<string>();
            if (prod.Files.Any())
            {
                foreach (var file in prod.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
                        }
                        newFiles.Add(fileName);
                    }
                }
            }
            Product product = prod.Adapt<Product>();
            await _unitOfWork.ProductRepository.CreateAsync(product);
            await _unitOfWork.CommitAsync();
            if (newFiles.Count > 0)
            {
                foreach (var file in newFiles)
                {
                    await _unitOfWork.ProductImagesRepository.CreateAsync(new ProductImages()
                    {
                        ImageUrl = file,
                        ProductId = product.Id
                    });
                }
                await _unitOfWork.CommitAsync();
            }
            return Created($"{Request.Scheme}://{Request.Host}/api/Product/{product.Id}", product.Adapt<ProductReqDTO>());
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct([FromRoute] int id, [FromForm] ProductReqDTO prod)
        {
            var productDB = _unitOfWork.ProductRepository.GetOne(e => e.Id == id, tracked: false);
            prod.Id = productDB.Id;
            List<string> newFiles = new List<string>();
            if (prod.Files.Any())
            {
                List<ProductImages> prodImgDB = _unitOfWork.ProductImagesRepository.Get(e => e.ProductId == id).ToList();
                foreach (var file in prod.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
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
                    });
                }
                await _unitOfWork.CommitAsync();

            }

            Product product = prod.Adapt<Product>();
            _unitOfWork.ProductRepository.Alter(product);
            await _unitOfWork.CommitAsync();
            return NoContent();

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync( [FromRoute] int id)
        {
            var prod = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
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
            await _unitOfWork.CommitAsync();
            return NoContent();
        }

    }
}
