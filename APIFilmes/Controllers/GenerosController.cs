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
    public class GenerosController : ControllerBase
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
        public GenerosController(APIFilmesDbContext context, IMapper mapper)
        {
            // Seta na variavel private local o context injetado pela dependência
            _context = context;

            // Seta na variavel private local o IMapper injetado pela dependência
            _mapper = mapper;
        }

        /// <summary>
        /// Método responsável pela listagem de todos os gêneros cadastrados
        /// </summary>
        /// <returns>Uma lista com os gêneros cadastrados no banco de dados</returns>
        [HttpGet]
        public ActionResult<IEnumerable<GeneroDTO>> ListarTodosGeneros()
        {
            try
            {
                /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
                * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
                 * */

                // Retorna a lista de gêneros encontrados no banco de dados
                var generos = _context.Generos.AsNoTracking().ToList();

                // Faz o mapeamento dos dados retornados do banco de dados para o DTO de Gêneros
                var generosDto = _mapper.Map<List<GeneroDTO>>(generos);

                return generosDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao listar os gêneros cadastrados. Tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável pela listagem de todos os gêneros cadastrados, com seus respectivos
        /// filmes vinculados
        /// </summary>
        /// <returns>Uma lista com os gêneros cadastrados no banco de dados e seus filmes vinculados</returns>
        [HttpGet("filmes")]
        public ActionResult<IEnumerable<GeneroDTO>> ListarTodosGenerosComFilmes()
        {
            try
            {
                // Retorna a lista de gêneros encontrados no banco de dados, incluindo seus filmes vinculados
                var generosComFilmes = _context.Generos.Include(f => f.Filmes).ToList();

                // Faz o mapeamento dos dados retornados do banco de dados para o DTO de Gêneros e seus filmes
                var generosDto = _mapper.Map<List<GeneroDTO>>(generosComFilmes);

                return generosDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao listar os gêneros cadastrados com seus filmes vinculados. Tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável por buscar um gênero específico no banco de dados,
        /// usando seu ID como condicional
        /// </summary>
        /// <param name="id">O ID do gênero a ser consultado</param>
        /// <returns>O objeto completo do gênero, caso seja encontrado algum com o ID informado OU código 404 caso nenhum seja encontrado</returns>
        [HttpGet("{id}", Name = "BuscarGenero")]
        public ActionResult<GeneroDTO> BuscarGenero(int id)
        {
            try
            {
                /* O AsNoTracking é utilizado para performance, por se tratar de uma consulta simples, não há necessidade de 
                     * mapear as alterações destes objetos buscados, haja visto que eles não serão alterados por esse recurso
                     * */

                // Busca no banco um gênero passando como condição o ID recebido via requisição
                var generoDb = _context.Generos.AsNoTracking().FirstOrDefault(f => f.ID.Equals(id));

                // Verifica se foi encontrado algum gênero, caso não seja encontrado, retornará um HTTP Not Found (404) com a mensagem para o usuário
                if (generoDb == null)
                {
                    return NotFound($"O Gênero com ID {id} não foi encontrado!");
                }

                // Faz o mapeamento do gênero encontrado no banco pelo ID para o DTO
                var generoDto = _mapper.Map<GeneroDTO>(generoDb);

                // Retorna o gênero encontrado no banco de dados pelo ID
                return generoDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu ao buscar o gênero
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar obter os dados do gênero com ID {id}. Tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável pelo cadastro de novos gêneros no banco de dados
        /// </summary>
        /// <param name="genero">O objeto Genero, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CadastrarGenero([FromBody] GeneroDTO generoDTO)
        {
            try
            {
                // Mapeia o DTO enviado para a Model de domínio
                var genero = _mapper.Map<Genero>(generoDTO);

                // Seta o Ativo e Data de Criação
                genero.Ativo = 1;
                genero.DataCriacao = DateTime.Now;

                // Adiciona em memória o objeto enviado no body da requisição
                _context.Generos.Add(genero);

                // Executa a query no BD, salvando o novo gênero
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var generoDto = _mapper.Map<GeneroDTO>(genero);

                // Retornamos o recurso completo, com a URL e detalhes de como consultar o mesmo
                return new CreatedAtRouteResult("BuscarGenero", new { id = generoDto.ID }, generoDto);
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar criar um novo gênero. Verifique os dados informados e tente novamente.");
            }
        }

        /// <summary>
        /// Método responsável pela atualização de gêneros no banco de dados
        /// </summary>
        /// <param name="id">O ID do gênero a ser alterado</param>
        /// <param name="genero">O objeto Genero, enviado no Body da requisição HTTP</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult AlterarGenero(int id, [FromBody] GeneroDTO generoDTO)
        {
            try
            {
                // Valida se o ID enviado na URL é o mesmo enviado no corpo do objeto
                if (id != generoDTO.ID)
                {
                    return BadRequest($"Não foi possível atualizar o Gênero com ID {id}, pois o ID informado como parâmetro difere do ID enviado no corpo da requisição. Verifique os dados e tente novamente.");
                }

                // Mapeia o DTO enviado para a Model de domínio
                var genero = _mapper.Map<Genero>(generoDTO);

                // Seta o estado do contexto como modificado
                _context.Entry(genero).State = EntityState.Modified;

                // Ignora os campo DataCriacao e Ativo, pois ele não deve ser atualizado pelo PUT
                _context.Entry(genero).Property(x => x.DataCriacao).IsModified = false;
                _context.Entry(genero).Property(x => x.Ativo).IsModified = false;

                // Salva as alterações no banco de dados
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var generoDto = _mapper.Map<GeneroDTO>(genero);

                // Retorna o objeto atualizado
                return Ok(generoDto);
            }
            catch (DbUpdateConcurrencyException) //Tratamento para se caso seja passado um ID inexistente
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"O Gênero com ID {id} não existe no banco de dados. Verifique os dados informados e tente novamente.");
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar o Gênero com ID {id}. Verifique os dados informados e tente novamente.");
            }
        }

        /// <summary>
        /// Método responsavel pela exclusão de Gêneros no banco de dados
        /// </summary>
        /// <param name="id">O ID do gênero a ser excluído</param>
        /// <returns>O objeto que foi excluído OU erro 404 se não for encontrado na base de dados</returns>
        [HttpDelete("{id}")]
        public ActionResult<GeneroDTO> DeletarGenero(int id)
        {
            try
            {
                // Busca o item no banco de dados, para garantir que existe
                var generoDb = _context.Generos.Find(id);

                // Se não encontrar pela chave, retorna Not Found pro usuário
                if (generoDb == null)
                {
                    return NotFound($"O Gênero com ID {id} não foi encontrado!");
                }

                // Se encontrar, remove do contexto e aplica o delete no banco de dados
                _context.Generos.Remove(generoDb);
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Genero para GeneroDTO
                var generoDto = _mapper.Map<GeneroDTO>(generoDb);

                // Retorna o genero que foi excluído do banco de dados
                return generoDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar o Gênero com ID {id}. Verifique os dados informados novamente.");
            }
        }

        /// <summary>
        /// Método responsável pro inativar um Gênero
        /// </summary>
        /// <param name="id">O ID do gênero a ser inativado</param>
        /// <returns>O Gênero que foi inativado</returns>
        [HttpPatch("inativar/{id}")]
        public ActionResult<GeneroDTO> InativarGenero(int id)
        {
            try
            {
                // Busca o item no banco de dados, para garantir que existe
                var generoDb = _context.Generos.Find(id);

                // Se não encontrar pela chave, retorna Not Found pro usuário
                if (generoDb == null)
                {
                    return NotFound($"O Gênero com ID {id} não foi encontrado!");
                }

                // Seta o estado do contexto como modificado
                _context.Entry(generoDb).State = EntityState.Modified;

                // Ignora o campo DataCriacao, pois ele não deve ser atualizado pelo PATCH
                _context.Entry(generoDb).Property(x => x.DataCriacao).IsModified = false;

                // Seta o campo Ativo para 0, inativando o Gênero
                _context.Entry(generoDb).Property(x => x.Ativo).CurrentValue = 0;

                // Persiste os dados no BD
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Genero para GeneroDTO
                var generoDto = _mapper.Map<GeneroDTO>(generoDb);

                // Retorna o genero alterado
                return generoDto;
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar inativar o Gênero com ID {id}. Verifique os dados informados novamente.");
            }
        }
    }
}
