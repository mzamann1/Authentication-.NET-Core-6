using JwtBearer.Constants;
using JwtBearer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtBearer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _app;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, IHostEnvironment host)
        {
            _logger = logger;
            _configuration = configuration;
            _app = host;
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

        public IActionResult Authenticate()
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, "some_id"), // no instradtruce right now
                new Claim("zepcom", "cookie")// no instradtruce right now
            };
            var constants = _configuration.GetSection("JwtConstants").Get<JWTConstants>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(constants?.Secret));

            var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(constants?.Issuer, constants?.Audiance, claims, notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1), signinCredentials);

            var tokenJson=new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenJson });

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);
            return Ok(Encoding.UTF8.GetString(bytes));
        }
    }
}