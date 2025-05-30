using Microsoft.AspNetCore.Mvc;
using BirthdayGifts.Web.Models.ViewModels.Account;
using BirthdayGifts.Services.Interfaces.Authentication;
using BirthdayGifts.Services.DTOs.Authentication;
using System.Threading.Tasks;
using System.Linq;

namespace BirthdayGifts.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Request body is null");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginRequest = new LoginRequest
            {
                Username = model.Username,
                Password = model.Password
            };

            var result = await _authenticationService.LoginAsync(loginRequest);

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.EmployeeId.Value);
                HttpContext.Session.SetString("UserName", result.FullName);

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
