using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public IActionResult Brands()
        {
            var brands = _unitOfWork.BrandRepository.Get();
            if (brands == null)
            {
                return NotFound("No brands found");
            }
            return Ok(brands);
        }
    }
}
