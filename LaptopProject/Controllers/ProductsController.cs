using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

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
        public IActionResult GetAll([FromQuery] int PageNum = 1 , [FromQuery] int pagesize = 30)
        {
            var products = _unitOfWork.ProductRepository.Get().Skip((PageNum-1)* pagesize).Take(pagesize).ToList();
            return Ok(products.Adapt<List<ProductReqDTO>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetOneProduct( [FromRoute] int id)
        {
            var prod = _unitOfWork.ProductRepository.GetOne(e => e.Id == id);
            if (prod == null)
                return NotFound();
            return Ok(prod.Adapt<ProductReqDTO>());
        }

        [HttpPost("")]
        public IActionResult CreateProduct([FromBody] ProductReqDTO prod)
        {
            
            Product product = prod.Adapt<Product>();
            _unitOfWork.ProductRepository.Create(product);
            _unitOfWork.Commit();
            return Created($"{Request.Scheme}://{Request.Host}/api/Product/{product.Id}", product.Adapt<ProductReqDTO>());
        }

    }
}
