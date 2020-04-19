using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TicTacToeCSharpPlayground.Business;
using TicTacToeCSharpPlayground.Configuration;
using TicTacToeCSharpPlayground.Repository;

namespace TicTacToeCSharpPlayground
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
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
            services.AddControllers(options => { options.Filters.Add(new HttpExceptionFilter()); });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
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
