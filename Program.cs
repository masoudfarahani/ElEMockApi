using Acornima;
using ELE.MockApi.Components;
using ELE.MockApi.Controllers.Filters;
using ELE.MockApi.Core.Db;
using ELE.MockApi.Core.Service;
using ELE.MockApi.Shared;
using Jsbeautifier;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Newtonsoft.Json;
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
        builder.Services.AddControllers(options => {
            options.Filters.Add(new ContentTypeFilter());
        });

        builder.Services.AddScoped<EndpointService>();

        builder.Services.AddMudServices();

        builder.Services.AddEndpointsApiExplorer();
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

        app.MapControllers();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
