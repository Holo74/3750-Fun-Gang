using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Session;
using Assignment_1.Data;
using Assignment_1.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Assignment_1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Assignment_1Context") ?? throw new InvalidOperationException("Connection string 'Assignment_1Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	Seeder.Init(services);
}

	// Configure the HTTP request pipeline.
	if (!app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Home/Error");
		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		app.UseHsts();
	}


app.UseStaticFiles();

app.UseRouting();

//create storage interface
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
