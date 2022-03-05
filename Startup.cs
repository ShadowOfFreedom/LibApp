using LibApp.Data;
using LibApp.Models;
using LibApp.Repositories;
using LibApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using LibApp.Dtos;
using LibApp.Dtos.Validators;
using Microsoft.IdentityModel.Tokens;

namespace LibApp {
    public class Startup {
        const string Scheme = "Bearer";

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);
            services.AddSingleton(authenticationSettings);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = Scheme;
                options.DefaultScheme = Scheme;
                options.DefaultChallengeScheme = Scheme;
            }).AddJwtBearer(configuration => {
                configuration.RequireHttpsMetadata = false;
                configuration.SaveToken = true;
                configuration.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                };
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IRepositoryAsync<Customer>, CustomersRepositoryAsync>();
            services.AddScoped<IRepositoryAsync<MembershipType>, MembershipTypesRepositoryAsync>();
            services.AddScoped<IRepositoryAsync<Book>, BooksRepositoryAsync>();
            services.AddScoped<IRepositoryAsync<Genre>, GenresRepositoryAsync>();
            services.AddScoped<IRepositoryAsync<Rental>, RentalsRepositoryAsync>();
            services.AddScoped<IAccountRepository, AccountsRepository>();
            services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
            services.AddScoped<IValidator<RegisterUserDTO>, RegisterUserDTOValidator>();

            services.AddControllersWithViews().AddFluentValidation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}