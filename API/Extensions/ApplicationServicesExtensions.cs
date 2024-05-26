using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Errors;

namespace API.Extensions
{
public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<StoreContext>(opt => 
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<ApiBehaviorOptions>(options =>
        {
                options.InvalidModelStateResponseFactory = actionContext =>
             {
                 var errors = actionContext.ModelState
                     .Where(e => e.Value.Errors.Count > 0)
                     .SelectMany(x => x.Value.Errors)
                     .Select(x => x.ErrorMessage).ToArray();

                 var errorResponse = new ApiValidationErrorResponse
             {
                     Errors = errors
             };

                 return new BadRequestObjectResult(errorResponse);
              };
        });
          services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                   policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"); 
                });
            });
            return services;
        }
    }
}