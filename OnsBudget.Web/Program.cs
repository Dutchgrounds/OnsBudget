using DevExpress.Blazor;
using MemBus;
using MemBus.Configurators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data;
using OnsBudget.Data.Entities;
using OnsBudget.Data.Services;
using OnsBudget.Web.Areas.Identity;

namespace OnsBudget.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


            builder.Services.AddDbContext<OnsBudgetDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
            builder.Services.AddDbContextFactory<OnsBudgetDbContext>( options => options.UseSqlServer( connectionString ), ServiceLifetime.Transient );
            
            builder.Logging.ClearProviders( );
            builder.Logging.AddLog4Net( );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<OnsBudgetDbContext>();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
            builder.Services.AddTransient<ImportService>( );
            builder.Services.AddTransient<TransactionService>( );
            builder.Services.AddTransient<CategoryService>( );
            builder.Services.AddScoped( ( sp ) => BusSetup.StartWith<Conservative>( ).Construct( ) );

            // Add DevExpress
            builder.Services.AddDevExpressBlazor( configure => configure.BootstrapVersion = BootstrapVersion.v5 );
            builder.WebHost.UseStaticWebAssets( );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
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

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}