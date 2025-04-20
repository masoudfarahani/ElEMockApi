using ELE.MockApi.Components;
using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.Service;
using ELE.MockApi.Shared;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Net.NetworkInformation;

namespace ELE.MockApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddDbContext<DataBaseContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        builder.Services.AddSingleton<AppEvents>();

        builder.Services.AddScoped<EndpointService>();

        builder.Services.AddMudServices();

        var app = builder.Build();

        // Apply pending migrations
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            db.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
