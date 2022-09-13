using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Models;
using Secret = IdentityServer4.Models.Secret;

namespace ApiClient2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            //retieve access_token
            var serverClient= _httpClientFactory.CreateClient();

            var discoveryDoc = await serverClient.GetDiscoveryDocumentAsync("https://localhost:7114/");

            var tokenResponse=await serverClient.RequestClientCredentialsTokenAsync( new ClientCredentialsTokenRequest {
                Address=discoveryDoc.TokenEndpoint,
                ClientId="client_id",
                ClientSecret= new Secret("client_secret".ToSha256()).ToString(),
                Scope="api1"
            });

            var apiClient= _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:7152/secret"); //api1's url
            var content=  await response.Content.ReadAsStringAsync();

            return Ok(new {
            acces_token=tokenResponse.AccessToken,
            message=content
            });
        }
    }
}
