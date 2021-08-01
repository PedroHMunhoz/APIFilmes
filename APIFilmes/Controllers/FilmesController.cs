using APIFilmes.Context;
using APIFilmes.DTOs;
using APIFilmes.Model;
using AutoMapper;
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

        // Variável para injeção da dependência do AutoMapper
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor da classe, usando para injetar a dependência do APIFilmesDbContext
        /// e vincular na variável local para uso dentro da controller
        /// </summary>
        /// <param name="context">Um objeto do tipo APIFilmesDbContext</param>
        public FilmesController(APIFilmesDbContext context, IMapper mapper)
        {
            // Seta na variavel private local o context injetado pela dependência
            _context = context;

            // Seta na variavel private local o IMapper injetado pela dependência
            _mapper = mapper;
        }

        /// <summary>
        /// Método responsável pela listagem de todos os filmes cadastrados
        /// </summary>
        /// <returns>Uma lista com os filmes cadastrados no banco de dados</returns>
        [HttpGet]
        public ActionResult<IEnumerable<FilmeDTO>> ListarTodosFilmes()
        {
            try
            {
                /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
                 * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
                 */

                // Retorna a lista de filmes encontrados no banco de dados
                var filmes = _context.Filmes.AsNoTracking().ToList();

                // Faz o mapeamento dos dados retornados do banco de dados para o DTO de Filmes
                var filmesDTO = _mapper.Map<List<FilmeDTO>>(filmes);

                // Retorna o DTO mapeado com os dados
                return filmesDTO;
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
        public ActionResult<FilmeDTO> BuscarFilme(int id)
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

                // Faz o mapeamento do filme encontrado no banco pelo ID para o DTO
                var filmeDTO = _mapper.Map<FilmeDTO>(filmeDb);

                // Retorna o DTO mapeado com os dados
                return filmeDTO;
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
        public ActionResult CadastrarFilme([FromBody] FilmeDTO filmeDTO)
        {
            try
            {
                // Mapeia o DTO enviado para a Model de domínio
                var filme = _mapper.Map<Filme>(filmeDTO);

                // Seta o Ativo e Data de Criação, pois é um cadastro novo
                filme.Ativo = 1;
                filme.DataCriacao = DateTime.Now;

                // Adiciona no contexto o objeto mapeado
                _context.Filmes.Add(filme);

                // Executa a query no BD, salvando o novo filme
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var filmeDto = _mapper.Map<FilmeDTO>(filme);

                // Retornamos o recurso completo, com a URL e detalhes de como consultar o mesmo
                return new CreatedAtRouteResult("BuscarFilme", new { id = filme.ID }, filmeDto);
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
        public ActionResult AlterarFilme(int id, [FromBody] FilmeDTO filmeDTO)
        {
            try
            {
                // Valida se o ID enviado na URL é o mesmo enviado no corpo do objeto
                if (id != filmeDTO.ID)
                {
                    return BadRequest($"Não foi possível atualizar o Filme com ID {id}, pois o ID informado como parâmetro difere do ID enviado no corpo da requisição. Verifique os dados e tente novamente.");
                }

                // Mapeia o DTO enviado para a Model de domínio
                var filme = _mapper.Map<Filme>(filmeDTO);

                // Seta o estado do contexto como modificado
                _context.Entry(filme).State = EntityState.Modified;

                // Ignora o campo DataCriacao e Ativo, pois ele não deve ser atualizado pelo PUT
                _context.Entry(filme).Property(x => x.DataCriacao).IsModified = false;
                _context.Entry(filme).Property(x => x.Ativo).IsModified = false;

                // Salva as alterações no banco de dados
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var filmeDto = _mapper.Map<FilmeDTO>(filme);

                // Retorna o objeto atualizado
                return Ok(filmeDto);
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
        public ActionResult<FilmeDTO> DeletarFilme(int id)
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

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var filmeDto = _mapper.Map<FilmeDTO>(filmeDb);

                // Retorna o filme que foi excluído do banco de dados
                return filmeDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar o Filme com ID {id}. Verifique os dados informados novamente.");
            }
        }

        /// <summary>
        /// Método responsável pro inativar um Filme
        /// </summary>
        /// <param name="id">O ID do filme a ser inativado</param>
        /// <returns>O Filme que foi inativado</returns>
        [HttpPatch("inativar/{id}")]
        public ActionResult<FilmeDTO> InativarFilme(int id)
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

                // Seta o estado do contexto como modificado
                _context.Entry(filmeDb).State = EntityState.Modified;

                // Ignora o campo DataCriacao, pois ele não deve ser atualizado pelo PATCH
                _context.Entry(filmeDb).Property(x => x.DataCriacao).IsModified = false;

                // Seta o campo Ativo para 0, inativando o Filme
                _context.Entry(filmeDb).Property(x => x.Ativo).CurrentValue = 0;

                // Persiste os dados no BD
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var filmeDto = _mapper.Map<FilmeDTO>(filmeDb);                

                // Retorna o filme alterado
                return filmeDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar inativar o Filme com ID {id}. Verifique os dados informados novamente.");
            }
        }
    }
}
