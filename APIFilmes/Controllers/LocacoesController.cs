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
    public class LocacoesController : ControllerBase
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
        public LocacoesController(APIFilmesDbContext context, IMapper mapper)
        {
            // Seta na variavel private local o context injetado pela dependência
            _context = context;

            // Seta na variavel private local o IMapper injetado pela dependência
            _mapper = mapper;
        }

        [HttpPost]
        public ActionResult CadastrarLocacao([FromBody] Locacao locacao)
        {
            try
            {
                // Variável de controle para os IDs de filmes que não existirem
                string idsFilmesInexistentes = string.Empty;

                // Verifica se os filmes informados existem no banco de dados
                foreach (Filme filmeLocado in locacao.ListaFilmes)
                {
                    // Faz a consulta no banco pela chave (ID) do filme informado
                    var filmeDb = _context.Filmes.AsNoTracking().FirstOrDefault(f => f.ID.Equals(filmeLocado.ID));

                    // Se o filme não existir no banco, guarda o ID para informar na mensagem de retorno
                    if(filmeDb == null)
                    {
                        if (!string.IsNullOrWhiteSpace(idsFilmesInexistentes))
                            idsFilmesInexistentes += ", ";

                        idsFilmesInexistentes += filmeLocado.ID;
                    }
                }

                // Se houver IDs inexistentes preenchidos na variável de controle, retorna um Bad Request pro usuário com os IDs incorretos
                if (!string.IsNullOrWhiteSpace(idsFilmesInexistentes))
                {
                    return BadRequest($"Os filmes com ID {idsFilmesInexistentes} não existem na base de dados! Verifique as informações e tente novamente.");
                }

                // Seta a Data de Locação
                locacao.DataLocacao = DateTime.Now;

                // Adiciona no contexto o objeto mapeado
                _context.Locacoes.Add(locacao);

                // Executa a query no BD, salvando a locação
                _context.SaveChanges();

                // Validação para inicializar sempre uma nova lista de itens de locação
                if (locacao.ItensLocacao == null)
                    locacao.ItensLocacao = new List<LocacaoItem>();

                // Para cada filme enviado, adiciona 1 item de locação, com o ID da locação criada anteriormente
                foreach (Filme filmeLocado in locacao.ListaFilmes)
                {
                    locacao.ItensLocacao.Add(new LocacaoItem { LocacaoID = locacao.ID, FilmeID = filmeLocado.ID });
                }

                // Adiciona os itens da locação no contexto
                _context.LocacoesItem.AddRange(locacao.ItensLocacao);

                // Salva os itens de locação no BD
                _context.SaveChanges();

                // Faz o mapeamento reverso, do Filme para FilmeDTO
                var locacaoDTO = _mapper.Map<LocacaoDTO>(locacao);

                // Retornamos o objeto da locação
                return Ok(locacaoDTO);
            }
            catch (Exception)
            {
                // Em caso de algum erro, retornará um HTTP Status 500 com a mensagem de erro que ocorreu
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar registrar a locação. Verifique os dados informados e tente novamente.");
            }
        }
    }
}
