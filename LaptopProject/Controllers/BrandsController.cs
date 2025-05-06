using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using System.Threading;

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
        public async Task<IActionResult> CreateBrandAsync([FromBody] RequestBrandDTO brandd , CancellationToken cancellationToken)
        {
            Models.Brand brand = brandd.Adapt<Models.Brand>();
            await _unitOfWork.BrandRepository.CreateAsync(brand, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Created($"{Request.Scheme}://{Request.Host}/api/Brand/id" , brand.Adapt<ResponseBrandDTO>());
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrandAsync([FromRoute] int id, [FromBody] RequestBrandDTO brandd , CancellationToken cancellationToken)
        {
            var brandDB = _unitOfWork.BrandRepository.GetOne(e => e.Id == id , tracked:false);
            if (brandDB == null) return NotFound();
            brandd.Id=brandDB.Id;
            brandDB = brandd.Adapt<Brand>();
            _unitOfWork.BrandRepository.Alter(brandDB);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrandAsync([FromRoute]int id, CancellationToken cancellationToken)
        {
            var brandDB = _unitOfWork.BrandRepository.GetOne(e => e.Id == id);
            if (brandDB == null) return NotFound();
            _unitOfWork.BrandRepository.Delete(brandDB);
            await _unitOfWork.CommitAsync(cancellationToken);
            return NoContent();
        }
    }
}
