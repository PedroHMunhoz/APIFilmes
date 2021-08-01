using APIFilmes.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIFilmes.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutorizacaoController : ControllerBase
    {
        // Variável para injeção de dependência do UserManager do Identity Model
        private readonly UserManager<IdentityUser> _userManager;

        // Variável para injeção de dependência do SignInManager do Identity Model
        private readonly SignInManager<IdentityUser> _signInManager;

        // Variável para injeção de dependência do IConfiguration, para leitura dos dados do JSON de configuração
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor da classe, usado para injetar as dependências necessárias da controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        public AutorizacaoController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            // Seta na variavel private local o UserManager injetado pela dependência
            _userManager = userManager;

            // Seta na variavel private local o SignInManager injetado pela dependência
            _signInManager = signInManager;

            // Seta na variavel private local o Configuration injetado pela dependência
            _configuration = configuration;
        }

        /// <summary>
        /// Método responsável pela geração do Token JWT que será usado para a autenticação do usuário
        /// </summary>
        /// <param name="usuario">O objeto do Usuário, que foi enviado via Body da requisição</param>
        /// <returns></returns>
        private UsuarioTokenDTO GeraToken(UsuarioDTO usuario)
        {
            // Array de Claims que serão utilizadas para geração do token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),                
            };

            // Chave secreta utilizada como base para geração do Token JWT, lida a partir do arquivo de configuração
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            // Instancia um objeto de credenciais passando a chave secreta da API
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Lê a quantidade de horas até expiração do token que foi gerado
            var tempoExpiracaoToken = _configuration["TokenConfiguration:HorasDuracaoToken"];

            // Data de expiração do token, considerando a hora atual acrescida do tempo configurado
            DateTime expiracao = DateTime.UtcNow.AddHours(double.Parse(tempoExpiracaoToken));

            // Instancia um objeto do JWT Token, passando as informações necessárias para validação posterior nas requisições
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["TokenConfiguration:Emissor"],
                audience: _configuration["TokenConfiguration:Publico"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais);

            // Retorna o DTO de UsuarioToken, com informações da geração e também o valor do token gerado
            return new UsuarioTokenDTO()
            {
                Autenticado = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                DataExpiracao = expiracao,
                MensagemAuth = "Token JWT gerado com sucesso"
            };
        }

        /// <summary>
        /// Método responsável por registrar um novo usuário na API
        /// </summary>
        /// <param name="usuarioDTO">O objeto Usuario enviado a partir do Body da requisição</param>
        /// <returns>Returna o token gerado, em caso de sucesso ou o erro, em caso de falha</returns>
        [HttpPost("registrar")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            // Cria um objeto do Identityyser com os dados informados pelo POST
            var usuario = new IdentityUser
            {
                UserName = usuarioDTO.Email,
                Email = usuarioDTO.Email,
                EmailConfirmed = true
            };

            // Chama e aguarda o retorno do método assíncrono que faz a criação do novo usuário no banco
            var resultado = await _userManager.CreateAsync(usuario, usuarioDTO.Password);

            // Se ocorreu algum erro na criação, retornao o BadRequest ao usuário, informando o erro
            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            // Aguarda a execução do login do usuário recém criado
            await _signInManager.SignInAsync(usuario, false);

            // Gera o token para o usuário recém registrado
            UsuarioTokenDTO usuarioTokenDTO = GeraToken(usuarioDTO);

            // Retorna o objeto do UsuarioTokenDTO com token gerado e demais informações da autenticação
            return Ok(usuarioTokenDTO);
        }

        /// <summary>
        /// Método responsável por fazer login de um usuário já existente na API
        /// </summary>
        /// <param name="usuarioDTO">O objeto Usuario enviado a partir do Body da requisição</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO usuarioDTO)
        {
            // Executa assincronamente o método de login passando os dados do usuário informados na requisição
            var resultado = await _signInManager.PasswordSignInAsync(usuarioDTO.Email, usuarioDTO.Password, false, false);

            // Se foi encontrado e autenticado corretamente
            if (resultado.Succeeded)
            {
                // Gera o token para o usuário recém registrado
                UsuarioTokenDTO usuarioTokenDTO = GeraToken(usuarioDTO);

                // Retorna o objeto do UsuarioTokenDTO com token gerado e demais informações da autenticação
                return Ok(usuarioTokenDTO);
            }
            else // Se não, retorna o erro ao usuário
            {
                ModelState.AddModelError(string.Empty, "Login inválido. Verifique suas credenciais e tente novamente.");
                return BadRequest(ModelState);
            }
        }        

        [HttpGet]
        public ActionResult HomePageAPI()
        {
            return Ok("Página Inicial API Filmes. Para consumir a API, sigas as instruções do arquivo Readme.md");
        }
    }
}
