using System.Configuration;
using AddressBook.Data;
using AddressBook.Services;
using AddressBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AddressBook;

public class Program
{
    public static IConfiguration Configuration { get; }
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(DataUtility.GetConnectionString(Configuration)));

        builder.Services.AddScoped<IImageService, BasicImageService>();

        var host = builder.Services.BuildServiceProvider();

        var dbContext = host.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();


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

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}