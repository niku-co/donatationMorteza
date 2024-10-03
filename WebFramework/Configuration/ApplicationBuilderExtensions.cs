using Common.Utilities;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.DataInitializer;

namespace WebFramework.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            Assert.NotNull(app, nameof(app));
            Assert.NotNull(env, nameof(env));

            /*if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }*/


            return app;
        }

        public static IApplicationBuilder InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            //Assert.NotNull(app, nameof(app));

            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>(); //Service locator

            ////dbContext.Database.EnsureDeleted();
            ////dbContext.Database.Migrate();

            dbContext?.Database.Migrate();

            var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>();
            foreach (var dataInitializer in dataInitializers)
                dataInitializer.InitializeData();

            return app;
        }
    }
}
