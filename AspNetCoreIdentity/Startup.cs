using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIdentity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreIdentity
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
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("ApplicationDbContextConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    // Default SignIn settings.
                    //Oturum açmak için onaylanmış bir "e-posta" nın gerekli olup olmadığını gösterir
                    options.SignIn.RequireConfirmedEmail = false;
                    //Oturum açmak için onaylanmış bir "telefon numarasının" gerekli olup olmadığını gösterir
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    //Oturum açmak için onaylanmış bir "hesabın" gerekli olup olmadığını gösterir
                    options.SignIn.RequireConfirmedAccount = true;//Default false

                    // Default Lockout settings.
                    //Kilitlenme meydana geldiğinde bir kullanıcının dışarıda bırakıldığı süre
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    //Kilitleme etkinleştirilmişse, bir kullanıcı kilitlenene kadar başarısız erişim girişimlerinin sayısı.
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    //Yeni bir kullanıcının kilitlenip kilitlenemeyeceğini belirler.
                    options.Lockout.AllowedForNewUsers = true;

                    // Default Password settings.
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;

                    // Default User settings.
                    //Kullanıcı adında izin verilen karakterler.
                    options.User.AllowedUserNameCharacters =
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    //Her kullanıcının benzersiz bir e - postaya sahip olmasını gerektirir.
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Login";
                options.LogoutPath = $"/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied";

                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
