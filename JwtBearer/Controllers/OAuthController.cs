using JwtBearer.Constants;
using JwtBearer.Models.OAuth;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JwtBearer.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize([FromQuery] OAuthQueryParams param)
        {
            var query = new QueryBuilder();
            query.Add(nameof(OAuthQueryParams.redirect_uri), param.redirect_uri??"");
            query.Add(nameof(OAuthQueryParams.state), param.state??"");
            return View(model:query?.ToString());
        }

        [HttpPost]
        public IActionResult Authorize([FromQuery] OAuthQueryParams param, string username)
        {
            const string code = "xyz";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add(nameof(OAuthQueryParams.state), param.state ?? "");
            return Redirect($"{param.redirect_uri}{query.ToString()}");
        }

        public async Task<IActionResult> Token (
            string grant_type, //flow of access_token request
            [FromQuery] OAuthQueryParams param,
            string code ,//confirmation of the authentication ,
            [FromServices] IConfiguration _configuration
            )
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

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseJson= JsonConvert.SerializeObject(new {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            });


            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            return Redirect(param.redirect_uri??"");
        }
    }
}
