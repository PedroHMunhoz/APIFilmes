using APIFilmes.Context;
using APIFilmes.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFilmes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Monta a configura��o do AutoMapper
            MapperConfiguration mappingConfiguration = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            // Cria a inst�ncia do AutoMapper
            IMapper mapper = mappingConfiguration.CreateMapper();

            // Registra o AutoMapper como servi�o, no formato Singleton, para termos sempre apenas uma inst�ncia dele
            services.AddSingleton(mapper);

            // Registra no servi�o o contexto para uso do banco de dados
            services.AddDbContext<APIFilmesDbContext>(config => config.UseSqlServer(Configuration.GetConnectionString("SQLConnection")));

            // Adiciona configura��o do Newtonsoft Json para ignorar a refer�ncia c�clica
            services.AddControllers().AddNewtonsoftJson(config => {
                config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
