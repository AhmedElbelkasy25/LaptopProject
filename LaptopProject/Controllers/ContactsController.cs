using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactsController(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var contacts = _unitOfWork.ContactUsRepository.Get(includeProps:e=>e.Include(e=>e.User)).ToList();
            if (contacts == null) return NotFound();

            var cntRes = contacts.Adapt<List<ContactUsResponse>>();
            for (int i = 0; i < contacts.Count; i++)
            {
                cntRes[i].UserName = contacts[i].User.Name;
                cntRes[i].UserEmail = contacts[i].User.Email;
            }
            return Ok(cntRes);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create( [FromBody] ContactUsRequest cnt , CancellationToken cancellationToken)
        {
            var Contact = cnt.Adapt<ContactUs>();
            Contact.ApplicationUserId= _userManager.GetUserId(User);
            await _unitOfWork.ContactUsRepository.CreateAsync(Contact, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return NoContent();
        }
    }
}
