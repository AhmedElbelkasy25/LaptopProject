using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactsController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var contacts = _unitOfWork.ContactUsRepository.Get().ToList();
            if(contacts == null) return NotFound();
            return Ok(contacts);
        }
    }
}
