using JwtBearer.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddAuthentication("OAuth").
    AddJwtBearer("OAuth", config =>
{
    var constants = builder.Configuration.GetSection("JwtConstants").Get<JWTConstants>();

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(constants?.Secret));

    /* access token is passing in query param*/
    config.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Query.ContainsKey("access_token"))
            {
                context.Token = context.Request.Query["access_token"];
            }
            return Task.CompletedTask;
        }
    };


    config.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = key,
        ValidIssuer = constants.Issuer,
        ValidAudience=constants.Audiance
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
