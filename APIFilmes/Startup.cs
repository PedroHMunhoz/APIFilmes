using APIFilmes.Context;
using APIFilmes.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            // Monta a configuração do AutoMapper
            MapperConfiguration mappingConfiguration = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            // Cria a instância do AutoMapper
            IMapper mapper = mappingConfiguration.CreateMapper();

            // Registra o AutoMapper como serviço, no formato Singleton, para termos sempre apenas uma instância dele
            services.AddSingleton(mapper);

            // Registra no serviço o contexto para uso do banco de dados
            services.AddDbContext<APIFilmesDbContext>(config => config.UseSqlServer(Configuration.GetConnectionString("SQLConnection")));

            // Registra no serviço o us do Identity para utilização dos tokens de autenticação
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<APIFilmesDbContext>().AddDefaultTokenProviders();

            // Registra no serviço a autenticação utilizando Bearer Token com JWT
            // Serão validados todos os parâmetros do Token: Issuer, Audience, Lifetime e Chave secreta
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Publico"],
                    ValidIssuer = Configuration["TokenConfiguration:Emissor"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))
                });

            // Adiciona configuração do Newtonsoft Json para ignorar a referência cíclica
            services.AddControllers().AddNewtonsoftJson(config =>
            {
                config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Adiciona o middleware para uso da autenticação
            app.UseAuthentication();

            // Adiciona o middleware para uso da autorização
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
