using APIFilmes.Context;
using APIFilmes.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIFilmes.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FilmesController : ControllerBase
    {
        // Variável para injeção de dependência do contexto
        private readonly APIFilmesDbContext _context;

        /// <summary>
        /// Construtor da classe, usando para injetar a dependência do APIFilmesDbContext
        /// e vincular na variável local para uso dentro da controller
        /// </summary>
        /// <param name="context">Um objeto do tipo APIFilmesDbContext</param>
        public FilmesController(APIFilmesDbContext context)
        {
            // Seta na variavel private local o context injetado pela dependência
            _context = context;
        }

        /// <summary>
        /// Método responsável pela listagem de todos os filmes cadastrados
        /// </summary>
        /// <returns>Uma lista com os filmes cadastrados no banco de dados</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Filme>> ListarTodosFilmes()
        {
            try
            {
                /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
                 * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
                 */

                // Retorna a lista de filmes encontrados no banco de dados
                return _context.Filmes.AsNoTracking().ToList();
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao listar os filmes cadastrados. Tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável por buscar um filme específico no banco de dados,
        /// usando seu ID como condicional
        /// </summary>
        /// <param name="id">O ID do filme a ser consultado</param>
        /// <returns>O objeto completo do filme, caso seja encontrado algum com o ID informado OU código 404 caso nenhum seja encontrado</returns>
        [HttpGet("{id}", Name = "BuscarFilme")]
        public ActionResult<Filme> BuscarFilme(int id)
        {
            try
            {
                /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
                 * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
                 */

                // Busca no banco um filme passando como condição o ID recebido via requisição
                var filmeDb = _context.Filmes.AsNoTracking().FirstOrDefault(f => f.ID.Equals(id));

                // Verifica se foi encontrado algum filme, caso não seja encontrado, retornará um HTTP Not Found (404)
                if (filmeDb == null)
                {
                    return NotFound($"O Filme com ID {id} não foi encontrado!");
                }

                // Retorna o filme encontrado no banco de dados pelo ID
                return filmeDb;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu ao buscar o gênero
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar obter os dados do filme com ID {id}. Tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável pelo cadastro de novos filmes no banco de dados
        /// </summary>
        /// <param name="filme">O objeto Filme, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CadastrarFilme([FromBody] Filme filme)
        {
            try
            {
                // Seta o Ativo e Data de Criação
                filme.Ativo = 1;
                filme.DataCriacao = DateTime.Now;

                // Adiciona em memória o objeto enviado no body da requisição
                _context.Filmes.Add(filme);

                // Executa a query no BD, salvando o novo filme
                _context.SaveChanges();

                // Retornamos o recurso completo, com a URL e detalhes de como consultar o mesmo
                return new CreatedAtRouteResult("BuscarFilme", new { id = filme.ID }, filme);
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar um novo filme. Verifique os dados informados e tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável pela atualização de filmes no banco de dados
        /// </summary>
        /// <param name="id">O ID do filme a ser alterado</param>
        /// <param name="filme">O objeto Filme, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult AlterarFilme(int id, [FromBody] Filme filme)
        {
            try
            {
                // Valida se o ID enviado na URL é o mesmo enviado no corpo do objeto
                if (id != filme.ID)
                {
                    return BadRequest($"Não foi possível atualizar o Filme com ID {id}, pois o ID informado como parâmetro difere do ID enviado no corpo da requisição. Verifique os dados e tente novamente.");
                }

                // Seta o estado do contexto como modificado e salva as alterações do objeto no banco de dados
                _context.Entry(filme).State = EntityState.Modified;
                _context.SaveChanges();

                // Retorna o objeto atualizado
                return Ok(filme);
            }
            catch (DbUpdateConcurrencyException) //Tratamento para se caso seja passado um ID inexistente
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"O Filme com ID {id} não existe no banco de dados. Verifique os dados informados e tente novamente.");
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar o Filme com ID {id}. Verifique os dados informados e tente novamente.");
            }
        }

        /// <summary>
        /// Método responsavel pela exclusão de Filmes no banco de dados
        /// </summary>
        /// <param name="id">O ID do filme a ser excluído</param>
        /// <returns>O objeto que foi excluído OU erro 404 se não for encontrado na base de dados</returns>
        [HttpDelete("{id}")]
        public ActionResult<Filme> DeletarFilme(int id)
        {
            try
            {
                // Busca o item no banco de dados, para garantir que existe
                var filmeDb = _context.Filmes.Find(id);

                // Se não encontrar pela chave, retorna Not Found pro usuário
                if (filmeDb == null)
                {
                    return NotFound($"O Filme com ID {id} não foi encontrado!");
                }

                // Se encontrar, remove do contexto e aplica o delete no banco de dados
                _context.Filmes.Remove(filmeDb);
                _context.SaveChanges();

                // Retorna o genero que foi excluído do banco de dados
                return filmeDb;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar o Filme com ID {id}. Verifique os dados informados novamente.");
            }
        }
    }
}
