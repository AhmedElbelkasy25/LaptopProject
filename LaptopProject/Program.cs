using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using DataAccess.DbIntializer;
namespace LaptopProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //add Cors
            builder.Services.AddCors(options =>
            { options.AddPolicy("MyAllowSpecifications",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity with ApplicationUser
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //DbIntializer
            builder.Services.AddScoped<IDbIntializer, DbIntializer>();


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            var app = builder.Build();

            //run DbIntializer
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDbIntializer>();
            service?.Intialize();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            //Add my cors
            app.UseCors("MyAllowSpecifications");

            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
