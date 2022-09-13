using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer4Example
{
    public static class Configuration
    {
        public static IEnumerable<ApiResource> GetApis() => new List<ApiResource>
        {
            new ApiResource("api1"),
        };

        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId="client_id",
                ClientSecrets={new Secret("client_secret".ToSha256())},
                AllowedGrantTypes=GrantTypes.ClientCredentials,
                AllowedScopes={"api1"}
            }
        };
    }
}
