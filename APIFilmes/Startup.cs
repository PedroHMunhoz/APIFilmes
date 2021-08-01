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

            // Registra no servi�o o us do Identity para utiliza��o dos tokens de autentica��o
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<APIFilmesDbContext>().AddDefaultTokenProviders();

            // Registra no servi�o a autentica��o utilizando Bearer Token com JWT
            // Ser�o validados todos os par�metros do Token: Issuer, Audience, Lifetime e Chave secreta
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

            // Adiciona configura��o do Newtonsoft Json para ignorar a refer�ncia c�clica
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

            // Adiciona o middleware para uso da autentica��o
            app.UseAuthentication();

            // Adiciona o middleware para uso da autoriza��o
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
