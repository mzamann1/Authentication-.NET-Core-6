using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication( config =>
{
    //we check the cookie to confirm that we are authenticated
    config.DefaultAuthenticateScheme = "ClientCookie";
    //when we sign in we will deal out a cookie
    config.DefaultSignInScheme = "ClientCookie";

    //use this to check if we are allowed to do something.
    config.DefaultChallengeScheme = "OurServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OurServer",config =>
    {
        config.ClientId = "client_id";
        config.ClientSecret = "client_secret";
        config.CallbackPath = "/oauth/callback";
        config.AuthorizationEndpoint = "https://localhost:7225/oauth/authorize";
        config.TokenEndpoint = "https://localhost:7225/oauth/token";
        config.SaveTokens = true;
        config.Events = new OAuthEvents()
        {
            OnCreatingTicket = context =>
            {
                var access_token=context.AccessToken;
                var payload = access_token?.Split('.')[1];
                var bytes = Convert.FromBase64String(payload);
                var jsonPayload=Encoding.UTF8.GetString(bytes);

                var claims=JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                foreach (var claim in claims)
                {
                    context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
