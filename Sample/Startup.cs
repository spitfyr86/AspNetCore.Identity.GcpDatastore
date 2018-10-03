using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Models;
using AspNetCore.Identity.GcpDatastore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Sample.Data;
using Sample.Services;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<Microsoft.AspNetCore.Identity.IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services
                .Configure<GcpDatastoreOption>(Configuration.GetSection("GcpDatastoreOption"))
                .AddGcpDatastoreDatabase()
                .AddGcpDatastoreDbContext<ApplicationUser, AspNetCore.Identity.GcpDatastore.IdentityRole>()
                .AddGcpDatastoreStore<ApplicationUser, AspNetCore.Identity.GcpDatastore.IdentityRole>();
            services.AddIdentity<ApplicationUser, AspNetCore.Identity.GcpDatastore.IdentityRole>()
                .AddDefaultTokenProviders();
           
            services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();
            services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();

            services.AddTransient<IEmailSender, EmailService>(i =>
                new EmailService(Configuration.GetSection("EmailConfig"))
            );

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
