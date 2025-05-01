using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandsController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [HttpGet("")]
        public IActionResult GetAllBrands()
        {
            var brands = _unitOfWork.BrandRepository.Get().ToList();
            
            return Ok(brands.Adapt<List<ResponseBrandDTO>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetBrand(int id)
        {
            var brand = _unitOfWork.BrandRepository.GetOne(filter:e=>e.Id == id);
            if (brand == null) return NotFound();
            return Ok(brand.Adapt<ResponseBrandDTO>());
        }

        [HttpPost("")]
        public IActionResult CreateBrand([FromBody] RequestBrandDTO brandd)
        {
            Models.Brand brand = brandd.Adapt<Models.Brand>();
            _unitOfWork.BrandRepository.Create(brand);
            _unitOfWork.CommitAsync();
            return Created($"{Request.Scheme}://{Request.Host}/api/Brand/id" , brand.Adapt<ResponseBrandDTO>());
        }
        [HttpPut("{id}")]
        public IActionResult UpdateBrand([FromRoute] int id, [FromBody] RequestBrandDTO brandd)
        {
            var brandDB = _unitOfWork.BrandRepository.GetOne(e => e.Id == id , tracked:false);
            if (brandDB == null) return NotFound();
            brandd.Id=brandDB.Id;
            brandDB = brandd.Adapt<Brand>();
            _unitOfWork.BrandRepository.Alter(brandDB);
            _unitOfWork.CommitAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand([FromRoute]int id)
        {
            var brandDB = _unitOfWork.BrandRepository.GetOne(e => e.Id == id);
            if (brandDB == null) return NotFound();
            _unitOfWork.BrandRepository.Delete(brandDB);
            _unitOfWork.CommitAsync();
            return NoContent();
        }
    }
}
