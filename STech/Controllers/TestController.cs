using Microsoft.AspNetCore.Mvc;
using STech.Services;

namespace STech.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailService _emailService;

        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            await _emailService.SendEmailAsync("lexaf50292@lofiey.com", "hè he", "hè hé", null, null);

            return Ok();
        }
    }
}
