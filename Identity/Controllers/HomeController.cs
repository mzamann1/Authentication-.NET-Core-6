using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NETCore.MailKit.Core;

namespace Identity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index");
        }
        ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {

        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password)
    {
        var user = new IdentityUser
        {
            UserName = username
        };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

            await _emailService.SendAsync("reciever@test.com", "email verify", link);

            // _userManager.ConfirmEmailAsync(u)
            return RedirectToAction(nameof(EmailVerification));
        }
        return RedirectToAction("Index");
    }
    public IActionResult Logout()
    {
        _signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }

    public IActionResult EmailVerification()
    {
        return View();
    }

    public async Task<IActionResult> VerifyEmail(string userId, string code)
    {
        var user= await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest();
        
        var result=await _userManager.ConfirmEmailAsync(user, code);
        if(result.Succeeded)
            return View();

        return BadRequest();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
