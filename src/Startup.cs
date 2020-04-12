using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.Business;
using src.Repository;

namespace src
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
            services.AddControllers();
            services.AddEntityFrameworkNpgsql().AddDbContext<CSharpPlaygroundContext>(optionsBuilder =>
            {
                var connectionString = Configuration.GetConnectionString("connectionDetails");
                optionsBuilder.UseNpgsql(connectionString);
            });

            // Repositories
            services.AddScoped<ITicTacToeRepository, TicTacToeRepository>();
            // Businesses
            services.AddSingleton<IBoardJudge, BoardJudge>();
            services.AddScoped<IBoardDealer, BoardDealer>();
            services.AddScoped<IGameDealer, GameDealer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CSharpPlaygroundContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            dataContext.Database.Migrate();
        }
    }
}
