namespace JwtBearer.Models.OAuth
{
    public class OAuthQueryParams
    {
        public string? response_type { get; set; } //authorizatioon flow type
        public string? client_id { get; set; } 
        public string? redirect_uri { get; set; } //
        public string? scope { get; set; } //what information I want.
        public string? state { get; set; } //random string genrated for goin back to same client.
    }
}
