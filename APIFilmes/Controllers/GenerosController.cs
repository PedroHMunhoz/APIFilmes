using APIFilmes.Context;
using APIFilmes.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIFilmes.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        // Variável para injeção de dependência do contexto
        private readonly APIFilmesDbContext _context;

        /// <summary>
        /// Construtor da classe, usando para injetar a dependência do APIFilmesDbContext
        /// e vincular na variável local para uso dentro da controller
        /// </summary>
        /// <param name="context">Um objeto do tipo APIFilmesDbContext</param>
        public GenerosController(APIFilmesDbContext context)
        {
            // Seta na variavel private local o context injetado pela dependência
            _context = context;
        }

        /// <summary>
        /// Método responsável pela listagem de todos os gêneros cadastrados
        /// </summary>
        /// <returns>Uma lista com os gêneros cadastrados no banco de dados</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Genero>> ListarTodosGeneros()
        {
            /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
             * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
             * */

            // Retorna a lista de gêneros encontrados no banco de dados
            return _context.Generos.AsNoTracking().ToList();
        }

        /// <summary>
        /// Método responsável por buscar um gênero específico no banco de dados,
        /// usando seu ID como condicional
        /// </summary>
        /// <param name="id">O ID do gênero a ser consultado</param>
        /// <returns>O objeto completo do gênero, caso seja encontrado algum com o ID informado OU código 404 caso nenhum seja encontrado</returns>
        [HttpGet("{id}", Name = "BuscarGenero")]
        public ActionResult<Genero> BuscarGenero(int id)
        {
            /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
             * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
             * */

            // Busca no banco um gênero passando como condição o ID recebido via requisição
            var generoDb = _context.Generos.AsNoTracking().FirstOrDefault(f => f.ID.Equals(id));

            // Verifica se foi encontrado algum gênero, caso não seja encontrado, retornará um HTTP Not Found (404)
            if (generoDb == null)
            {
                return NotFound();
            }

            // Retorna o gênero encontrado no banco de dados pelo ID
            return generoDb;
        }

        /// <summary>
        /// Método responsável pelo cadastro de novos gêneros no banco de dados
        /// </summary>
        /// <param name="genero">O objeto Genero, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CadastrarGenero([FromBody] Genero genero)
        {
            // Seta o Ativo e Data de Criação
            genero.Ativo = 1;
            genero.DataCriacao = DateTime.Now;

            // Adiciona em memória o objeto enviado no body da requisição
            _context.Generos.Add(genero);

            // Executa a query no BD, salvando o novo gênero
            _context.SaveChanges();

            // Retornamos o recurso completo, com a URL e detalhes de como consultar o mesmo
            return new CreatedAtRouteResult("BuscarGenero", new { id = genero.ID }, genero);
        }

        /// <summary>
        /// Método responsável pela atualização de gêneros no banco de dados
        /// </summary>
        /// <param name="id">O ID do gênero a ser alterado</param>
        /// <param name="genero">O objeto Genero, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult AlterarGenero(int id, [FromBody] Genero genero)
        {
            // Valida se o ID enviado na URL é o mesmo enviado no corpo do objeto
            if (id != genero.ID)
            {
                return BadRequest();
            }

            // Seta o estado do contexto como modificado e salva as alterações do objeto no banco de dados
            _context.Entry(genero).State = EntityState.Modified;
            _context.SaveChanges();

            // Retorna o objeto atualizado
            return Ok(genero);
        }

        /// <summary>
        /// Método responsavel pela exclusão de Gêneros no banco de dados
        /// </summary>
        /// <param name="id">O ID do gênero a ser excluído</param>
        /// <returns>O objeto que foi excluído OU erro 404 se não for encontrado na base de dados</returns>
        [HttpDelete("{id}")]
        public ActionResult<Genero> DeletarGenero(int id)
        {
            // Busca o item no banco de dados, para garantir que existe
            var generoDb = _context.Generos.Find(id);

            // Se não encontrar pela chave, retorna Not Found pro usuário
            if (generoDb == null)
            {
                return NotFound();
            }

            // Se encontrar, remove do contexto e aplica o delete no banco de dados
            _context.Generos.Remove(generoDb);
            _context.SaveChanges();

            // Retorna o genero que foi excluído do banco de dados
            return generoDb;
        }
    }
}
