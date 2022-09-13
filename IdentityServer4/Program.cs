using IdentityServer4Example;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentityServer()
    .AddInMemoryApiResources(Configuration.GetApis())
    .AddInMemoryClients(Configuration.GetClients())
    .AddDeveloperSigningCredential();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseIdentityServer();

app.UseEndpoints(endpoints => { 
    endpoints.MapDefaultControllerRoute();
});


app.Run();